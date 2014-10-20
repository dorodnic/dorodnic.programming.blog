using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentInfrastructure
{
    public class AsyncLoop : ILoop
    {
        private const int MaxResponceMs = 1000;

        private readonly TimeSpan _interval;
        private readonly bool _threadBased;
        private long _keepWorking;
        private readonly ManualResetEvent _workerIsUp = new ManualResetEvent(false);
        private readonly ManualResetEvent _workerIsDown = new ManualResetEvent(false);
        private readonly ManualResetEvent _abortEvnt = new ManualResetEvent(false);
        private readonly ManualResetEvent _threadReady = new ManualResetEvent(false);
        private readonly object _syncObj = new object();
        private CancellationTokenSource _tokenSource;
        private Exception _relayException;

        private event Action<CancellationToken> _onLoop;
        private event Action _onStart;
        private event Action _onStop;
        private event Action<Exception> _onError;

        public AsyncLoop(Action<CancellationToken> action, 
            TimeSpan interval = new TimeSpan(), 
            bool threadBased = false)
        {
            _onLoop = action;
            _interval = interval;
            _threadBased = threadBased;
        }

        public void Dispose()
        {
            Stop();
        }


        private async void Loop()
        {
            _threadReady.Set();

            try
            {
                if (_onStart != null) _onStart();
            }
            catch (Exception ex)
            {
                _relayException = ex;
                _workerIsUp.Set();
                return;
            }

            _workerIsUp.Set();

            while (Interlocked.Read(ref _keepWorking) == 1)
            {
                try
                {
                    if (_onLoop != null) _onLoop(_tokenSource.Token);
                }
                catch (Exception ex)
                {
                    if (_onError != null) _onError(ex);
                }

                if (_threadBased)
                {
                    _abortEvnt.WaitOne(_interval);
                }
                else
                {
                    try
                    {
                        await Task.Delay(_interval, _tokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            }

            try
            {
                if (_onStop != null) _onStop();
            }
            catch (Exception ex)
            {
                _relayException = ex;
            }

            _workerIsDown.Set();
        }

        public ILoop BeforeStart(Action action)
        {
            _onStart += action;
            return this;
        }

        public ILoop BeforeStop(Action action)
        {
            _onStop += action;
            return this;
        }

        public ILoop WhenFailed(Action<Exception> action)
        {
            _onError += action;
            return this;
        }

        public ILoop Loop(Action<CancellationToken> action)
        {
            _onLoop += action;
            return this;
        }

        public ILoop Start()
        {
            lock (_syncObj)
            {
                if (Interlocked.Read(ref _keepWorking) == 0)
                {
                    _tokenSource = new CancellationTokenSource();

                    _keepWorking = 1;

                    if (_threadBased)
                    {
                        var thread = new Thread(Loop) { IsBackground = true };
                        thread.Start();
                    }
                    else
                    {
                        Task.Factory.StartNew(Loop);
                    }

                    _threadReady.WaitOne();
                    _threadReady.Reset();

                    if (!_workerIsUp.WaitOne(MaxResponceMs))
                    {
                        throw new Exception("Failed to start the Async-Thread!");
                    }
                    _workerIsUp.Reset();

                    RethrowException();
                }

                return this;
            }
        }

        private void RethrowException()
        {
            if (_relayException != null)
            {
                if (_onError != null) _onError(_relayException);
                else throw _relayException;
            }
        }

        public ILoop Stop()
        {
            lock (_syncObj)
            {
                if (Interlocked.Read(ref _keepWorking) == 1)
                {
                    Interlocked.Decrement(ref _keepWorking);

                    _tokenSource.Cancel(false);
                    _abortEvnt.Set();

                    if (_workerIsDown.WaitOne(MaxResponceMs))
                    {
                        _workerIsDown.Reset();
                        RethrowException();
                    }
                    else
                    {
                        throw new Exception("Failed to stop the Async-Thread!");
                    }
                }
                return this;
            }
        }
    }
}

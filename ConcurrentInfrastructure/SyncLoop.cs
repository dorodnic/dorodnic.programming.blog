using System;
using System.Threading;

namespace ConcurrentInfrastructure
{
    public class SyncLoop : ILoop
    {
        private event Action<CancellationToken> _onLoop;
        private event Action _onStart;
        private event Action _onStop;
        private event Action<Exception> _onError;
        private bool _running = false;

        public SyncLoop(Action<CancellationToken> action)
        {
            _onLoop = action;
        }

        public void Step(CancellationToken token)
        {
            if (!_running) return;

            try
            {
                _onLoop(token);
            }
            catch (Exception ex)
            {
                if (_onError != null) _onError(ex);
            }
        }

        public void Dispose()
        {
            if (_onLoop != null)
            {
                Stop();
                _onLoop = null;
            }
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
            if (!_running)
            {
                if (_onStart != null) _onStart();

                _running = true;
            }
            return this;
        }

        public ILoop Stop()
        {
            if (_running)
            {
                if (_onStop != null) _onStop();

                _running = false;
            }
            return this;
        }
    }
}

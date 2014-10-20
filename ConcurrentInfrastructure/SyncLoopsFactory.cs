using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentInfrastructure
{
    public class SyncLoopsFactory : ILoopsFactory
    {
        private readonly ConcurrentBag<SyncLoop> _loops 
            = new ConcurrentBag<SyncLoop>();

        private event Action<CancellationToken> _onLoop;
        private event Action _onStart;
        private event Action _onStop;
        private event Action<Exception> _onError;

        public ILoopsFactory BeforeStart(Action action)
        {
            _onStart += action;
            return this;
        }

        public ILoopsFactory BeforeStop(Action action)
        {
            _onStop += action;
            return this;
        }

        public ILoopsFactory WhenFailed(Action<Exception> action)
        {
            _onError += action;
            return this;
        }

        public ILoopsFactory Loop(Action<CancellationToken> action)
        {
            _onLoop += action;
            return this;
        }


        public ILoop Create(Action<CancellationToken> action, TimeSpan interval = new TimeSpan())
        {
            var loop = new SyncLoop(action).Loop(_onLoop).BeforeStart(_onStart).BeforeStop(_onStop).WhenFailed(_onError) as SyncLoop;
            _loops.Add(loop);
            return loop;
        }

        public void StepAll()
        {
            foreach (var loop in _loops)
            {
                var doneEvnt = new AutoResetEvent(false);

                var cancelSrc = new CancellationTokenSource();

                Task.Factory.StartNew(() =>
                {
                    doneEvnt.WaitOne(Debugger.IsAttached ? 10000 : 1);
                    cancelSrc.Cancel();
                });

                loop.Step(cancelSrc.Token);

                doneEvnt.Set();
            }
            
        }
    }
}

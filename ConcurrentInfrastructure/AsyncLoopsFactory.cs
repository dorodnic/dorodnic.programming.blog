using System;
using System.Threading;

namespace ConcurrentInfrastructure
{
    public class AsyncLoopsFactory : ILoopsFactory
    {
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

        public ILoop Create(Action<CancellationToken> action, 
            TimeSpan interval = new TimeSpan())
        {
            return new AsyncLoop(action, interval).Loop(_onLoop).BeforeStart(_onStart).BeforeStop(_onStop).WhenFailed(_onError);
        }
    }
}

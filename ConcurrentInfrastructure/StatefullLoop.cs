using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentInfrastructure
{
    class StatefullLoop<T> : IStatefullLoop<T>
    {
        private readonly ILoop _loop;
        private T _state;

        public StatefullLoop(ILoop loop)
        {
            _loop = loop;
        }

        public IStatefullLoop<T> Setup(Func<T> action)
        {
            _loop.BeforeStart(() =>
            {
                _state = action();
            });
            
            return this;
        }

        public IStatefullLoop<T> Teardown(Action<T> action)
        {
            _loop.BeforeStop(() =>
            {
                action(_state);
                _state = default(T);
            });
            return this;
        }

        public IStatefullLoop<T> WhenFailed(Action<Exception> action)
        {
            _loop.WhenFailed(action);
            return this;
        }

        public IStatefullLoop<T> Loop(Action<CancellationToken, T> action)
        {
            _loop.Loop(c => action(c, _state));
            return this;
        }

        public IStatefullLoop<T> Start()
        {
            _loop.Start();
            return this;
        }

        public IStatefullLoop<T> Stop()
        {
            _loop.Stop();
            return this;
        }
    }
}

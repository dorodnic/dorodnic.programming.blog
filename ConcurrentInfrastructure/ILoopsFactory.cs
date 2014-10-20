using System;
using System.Threading;

namespace ConcurrentInfrastructure
{
    public interface ILoopsFactory
    {
        ILoopsFactory BeforeStart(Action action);
        ILoopsFactory BeforeStop(Action action);
        ILoopsFactory WhenFailed(Action<Exception> action);
        ILoopsFactory Loop(Action<CancellationToken> action);

        ILoop Create(Action<CancellationToken> action, TimeSpan interval = new TimeSpan());
    }
}

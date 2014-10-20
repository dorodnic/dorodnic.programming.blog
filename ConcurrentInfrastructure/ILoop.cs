using System;
using System.Threading;

namespace ConcurrentInfrastructure
{
    public interface ILoop : IDisposable
    {
        ILoop BeforeStart(Action action);
        ILoop BeforeStop(Action action);
        ILoop WhenFailed(Action<Exception> action);
        ILoop Loop(Action<CancellationToken> action);

        ILoop Start();
        ILoop Stop();
    }
}

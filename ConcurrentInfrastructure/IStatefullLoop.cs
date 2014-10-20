using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentInfrastructure
{
    public interface IStatefullLoop<T>
    {
        IStatefullLoop<T> Setup(Func<T> action);
        IStatefullLoop<T> Teardown(Action<T> action);
        IStatefullLoop<T> WhenFailed(Action<Exception> action);

        IStatefullLoop<T> Start();
        IStatefullLoop<T> Stop();
    }
}

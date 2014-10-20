using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConcurrentInfrastructure
{
    public static class LoopsFactoryExtensions
    {
        public static ActiveObject<Action> CreateInvoker(this ILoopsFactory factory,
            TimeSpan interval = new TimeSpan())
        {
            return new ActiveObject<Action>(factory, a => a(), 0, interval);
        }

        public static ActiveObject<Action> CreateInvoker(this ILoopsFactory factory,
            int capacity,
            TimeSpan interval = new TimeSpan())
        {
            return new ActiveObject<Action>(factory, a => a(), capacity, interval);
        }

        public static IStatefullLoop<T> CreateWith<T>(this ILoopsFactory factory, Action<CancellationToken, T> action, TimeSpan timeout = new TimeSpan())
        {
            return new StatefullLoop<T>(factory.Create(c => { }, timeout)).Loop(action);
        }

        public static bool TryInvoke<T>(this IEnumerable<ActiveObject<T>> invokers, IEnumerable<T> prms)
        {
            var invokersList = invokers.ToList();
            if (invokersList.All(invoker => !invoker.IsFull()))
            {
                foreach (var pair in invokersList.Zip(prms,
                    (invoker, prm) => new
                    {
                        Invoker = invoker,
                        Param = prm
                    }))
                {
                    pair.Invoker.Invoke(pair.Param);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

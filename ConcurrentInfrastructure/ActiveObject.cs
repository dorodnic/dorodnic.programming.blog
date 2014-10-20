using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConcurrentInfrastructure
{
    public class ActiveObject<T> : IDisposable
    {
        private ILoop _loop;
        private BlockingCollection<Message> _queue;
        private Action<T> _action;
        private readonly int _capacity;
        private TimeSpan _interval;

        private class Message
        {
            public T Content { get; set; }
            public string CallStack { get; set; }
            public DateTime RequestTime { get; set; }
        }

        public ActiveObject(ILoopsFactory factory, Action<T> action, 
            int capacity,
            TimeSpan interval = new TimeSpan())
        {
            _loop = factory.Create(LoopAction, interval);
            _queue = new BlockingCollection<Message>(new ConcurrentQueue<Message>());
            _action = action;
            _capacity = capacity;
            _interval = interval;
        }

        public bool IsFull()
        {
            return _queue.Count() >= _capacity && _capacity > 0;
        }

        public bool TryInvoke(T param)
        {
            if (!IsFull())
            {
                _queue.Add(new Message
                {
                    Content = param,
                    CallStack = Environment.StackTrace,
                    RequestTime = DateTime.UtcNow
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Invoke(T param)
        {
            _queue.Add(new Message
            {
                Content = param,
                CallStack = Environment.StackTrace,
                RequestTime = DateTime.UtcNow
            });
        }

        private void LoopAction(CancellationToken token)
        {
            var queue = FetchAllItemsInQueue(token);

            while (queue.Count > 0)
            {
                if (token.IsCancellationRequested)
                {
                    while (queue.Any()) _queue.Add(queue.Dequeue());
                    return;
                }

                var t1 = queue.Dequeue();
                try
                {
                    _action(t1.Content);
                }
                catch (Exception ex)
                {
                    var newEx = new ActiveObjectException
                    {
                        Exception = ex,
                        RequestStackTrace = t1.CallStack,
                        RequestTime = t1.RequestTime
                    };
                    throw newEx;
                }
            }
        }

        private Queue<Message> FetchAllItemsInQueue(CancellationToken token)
        {
            var queue = new Queue<Message>();

            Message t;
            while (_queue.Count > 0 && _queue.TryTake(out t, (int) _interval.TotalMilliseconds, token))
            {
                if (token.IsCancellationRequested) return queue;

                queue.Enqueue(t);
            }
            return queue;
        }

        public void Dispose()
        {
            if (_loop != null)
            {
                Stop();
                _queue = null;
                _loop.Dispose();
                _loop = null;
                _action = null;
            }
        }

        public ActiveObject<T> BeforeStart(Action action)
        {
            _loop.BeforeStart(action);
            return this;
        }

        public ActiveObject<T> BeforeStop(Action action)
        {
            _loop.BeforeStop(action);
            return this;
        }

        public ActiveObject<T> WhenFailed(Action<Exception> action)
        {
            _loop.WhenFailed(action);
            return this;
        }

        public ActiveObject<T> Loop(Action<CancellationToken> action)
        {
            _loop.Loop(action);
            return this;
        }

        public ActiveObject<T> Start()
        {
            _loop.Start();
            return this;
        }

        public ActiveObject<T> Stop()
        {
            _loop.Stop();
            return this;
        }
    }
}

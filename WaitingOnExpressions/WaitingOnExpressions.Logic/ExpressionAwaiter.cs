using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WaitingOnExpressions.Logic
{
    public class ExpressionAwaiter : INotifyCompletion
    {
        private Action _continuation;

        public ExpressionAwaiter()
        {
            IsCompleted = false;
        }

        public bool IsCompleted { get; private set; }

        public void GetResult()
        {

        }

        public void Complete()
        {
            if (_continuation != null)
            {
                _continuation();
                IsCompleted = true;
            }
        }

        public void OnCompleted(Action continuation)
        {
            _continuation += continuation;
        }
    }

}

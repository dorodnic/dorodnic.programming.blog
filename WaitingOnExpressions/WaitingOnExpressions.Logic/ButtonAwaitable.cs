using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WaitingOnExpressions.Logic
{
    public class ButtonAwaitable
    {
        private readonly Button _target;

        public ButtonAwaitable(Button target)
        {
            _target = target;
        }

        public class ButtonAwaiter : INotifyCompletion
        {
            private readonly Button _target;
            private Action _continuation;

            public bool IsCompleted { get; set; }

            public ButtonAwaiter(Button target)
            {
                _target = target;
                _target.Click += TargetOnClick;
                IsCompleted = false;
            }

            public void OnCompleted(Action continuation)
            {
                _continuation += continuation;
            }

            public void GetResult()
            {
            }

            private void TargetOnClick(object sender, RoutedEventArgs routedEventArgs)
            {
                if (_continuation != null) _continuation();
                _target.Click -= TargetOnClick;
                IsCompleted = true;
            }
        }

        public ButtonAwaiter GetAwaiter()
        {
            return new ButtonAwaiter(_target);
        }
    }
}

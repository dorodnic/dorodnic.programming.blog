using System;

namespace GenericExtensions
{
    public class Condition<T> where T : class
    {
        public T Value { get; set; }
        public bool Handled { get; set; }

        public PartiallyEvaluatedCondition<T> Clone()
        {
            return new PartiallyEvaluatedCondition<T> { Handled = Handled, Value = Value };
        }

        public PartiallyEvaluatedCondition<T> Handle()
        {
            return new PartiallyEvaluatedCondition<T> { Handled = true, Value = Value };
        }

        public PartiallyEvaluatedCondition<T> Is<S>(Action<S> action)
            where S : class, T
        {
            if (Value == null)
            {
                return Clone();
            }
            if (Value is S)
            {
                if (action != null) action(Value as S);
                return Handle();
            }
            return Clone();
        }

        public PartiallyEvaluatedCondition<T> IsNull(Action action)
        {
            if (Value == null)
            {
                if (action != null) action();
                return Handle();
            }
            else
            {
                return Clone();
            }
        }

        public PartiallyEvaluatedCondition<T> IsNotNull(Action<T> action)
        {
            if (Value != null)
            {
                if (action != null) action(Value);
                return Handle();
            }
            else
            {
                return Clone();
            }
        }
    }

    public class PartiallyEvaluatedCondition<T> : Condition<T> where T : class
    {
        public void Else(Action action)
        {
            if (!Handled && action != null)
            {
                action();
            }
        }
    }
}
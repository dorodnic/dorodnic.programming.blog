namespace GenericExtensions
{
    public class Identity<T> where T : class
    {
        public T Value { get; set; }

        public bool Not<S>() where S : T
        {
            return !(Value is S);
        }

        public bool Null()
        {
            return Value == null;
        }

        public bool NotNull()
        {
            return Value != null;
        }
    }
}
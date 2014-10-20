namespace GenericExtensions
{
    public static class Extensions
    {
        public static Identity<T> Is<T>(this T _this) where T : class
        {
            return new Identity<T>
            {
                Value = _this
            };
        }

        public static Condition<T> If<T>(this T _this) where T : class
        {
            return new Condition<T> { Value = _this };
        }
    }
}
using System;
using System.Linq;

namespace ConsoleApplication3
{
    public static class Sequences
    {
        public static ISequence<IOption<int>> NaturalNumbers
        {
            get
            {
                return Generate<int>(() =>
                {
                    var curr = 0;
                    return () => curr++;
                });
            }
        }

        public static ISequence<IOption<T>> Generate<T>(Func<Func<T>> generator)
        {
            return new FunctionalSequence<IOption<T>>(() =>
            {
                var it = generator();
                return () => new Option<T>(it());
            });
        }

        public static ISequence<int> RandomNumbers
        {
            get
            {
                return new FunctionalSequence<int>(() =>
                {
                    var rand = new Random();
                    return rand.Next;
                });
            }
        }

        //public static ISequence<bool> RandomBits
        //{
        //    get { return from x in RandomNumbers select x % 2 == 0; }
        //}

        //public static ISequence<int> Primes
        //{
        //    get { return from x in NaturalNumbers where x.IsPrime() where x > 1 select x; }
        //}

        //public static bool IsPrime(this int x)
        //{
        //    return Enumerable.Range(2, (int)Math.Sqrt(x)).Any(y => x % y == 0);
        //}
    }
}
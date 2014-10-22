using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    class Program
    {
        static IEnumerable<int> Get()
        {
            var i = 0;
            while (true) yield return i++;
        }

        static void Main(string[] args)
        {
            var N = Get();


            

            var q = from x in Sequences.NaturalNumbers
                group x by x % 2;

Console.WriteLine(from x in Sequences.NaturalNumbers
                  group x by x % 2);

            //Console.WriteLine(q.At(0));
            //Console.WriteLine(q.At(1));

            //Console.WriteLine(q.ToString(15));

var NaturalNumbersRx = Observable.Generate(0, x => true, x => x + 1, x => x);

var rxQuery = from x in NaturalNumbersRx
                from y in (from y in NaturalNumbersRx where y < x select y)
                select "(" + x + ", " + y + ")";

using (var sub = rxQuery.Subscribe(
    Console.WriteLine, // On new item
    Console.WriteLine, // on error
    () => { } // On done
    ))
{
    Console.WriteLine("Press any key...");
    Console.ReadKey(); // exit and dispose of the subscription
}




            Console.WriteLine(from grouping in (from x in Sequences.NaturalNumbers group x by x % 2)
                              from x in grouping
                              select "(" + grouping.Key + ", " + x + ")");

            var q2 = from grouping in (from x in Sequences.NaturalNumbers group x by x % 2)
                     from x in grouping
                     select "(" + grouping.Key + ", " + x + ")";

            var composite = from x in Sequences.NaturalNumbers
                            from y in (from y in Sequences.NaturalNumbers where y < x select y)
                            select "(" + x + ", " + y + ")";

            var composite2 = from y in Sequences.NaturalNumbers
                             from x in (from x in Sequences.NaturalNumbers where y < x select x)
                             select "(" + y + ", " + x + ")";

            //Console.WriteLine(q2.ToString(20));

            foreach (var x in composite2.Take(100))
            {
                Console.WriteLine(x);
            }

            //Console.WriteLine(from x in Sequences.Primes
            //                      where x % 2 == 0 
            //                      from y in Sequences.Primes
            //                      where y > x
            //                      select x + y);

            //Console.WriteLine(from x in Sequences.Primes
            //                  from y in Sequences.Primes
            //                  where y == x + 2
            //                  select new { x, y });

            //var composite =   from x in Sequences.NaturalNumbers
            //                  from y in Sequences.NaturalNumbers
            //                  where y > 1 && y <= Math.Sqrt(x)
            //                  where x % y == 0
            //                  group x by x;

            //var primes = composite.Zip(Sequences.NaturalNumbers, (x, y) => (x.Key == y ? 0 : y));
        }
    }
}

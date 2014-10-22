using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    public static class NtoN2Mapping
    {
        public struct Pair
        {
            public override string ToString()
            {
                return "(" + I + ", " + J + ")";
            }

            public long I {get;set;}
            public long J { get; set; }
        }

        public static Pair GetPair(long n)
        {
            long i = 0;
            long j = 0;
            long k = 1;
            while (n > 0)
            {
                if ((n & 1) == 1)
                {
                    i = i + k;
                }
                n = n >> 1;
                if ((n & 1) == 1)
                {
                    j = j + k;
                }
                n = n >> 1;
                k = k << 1;
            }
            return new Pair { I = i, J = j };
        }
    }
}

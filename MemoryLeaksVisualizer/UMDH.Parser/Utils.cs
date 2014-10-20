using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UMDH.Parser
{
    public static class Utils
    {
        public static T AddOrGet<T>(this ICollection<T> _this, T obj)
        {
            if (!_this.Contains(obj))
            {
                _this.Add(obj);
            }
            return obj;
        }
    }
}

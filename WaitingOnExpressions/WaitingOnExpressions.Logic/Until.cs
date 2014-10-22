using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WaitingOnExpressions.Logic
{
    public static class Until
    {
        public static ExpressionAwaitable BecomesTrue(Expression<Func<bool>> expression)
        {
            return new ExpressionAwaitable(expression);
        }

        public static ButtonAwaitable IsClicked(Button target)
        {
            return new ButtonAwaitable(target);
        }
    }
}

using GenericExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Expression = System.Linq.Expressions.Expression;

namespace WaitingOnExpressions.Logic
{
    public class DependencyPropertyExtractor : ExpressionVisitor
    {
        public class DependencyPropertyInstance
        {
            public DependencyProperty Property { get; set; }
            public DependencyObject Owner { get; set; }
        }

        public IReadOnlyCollection<DependencyPropertyInstance> ExtractedItems { get { return _extractedItems; } }
        private readonly List<DependencyPropertyInstance> _extractedItems = new List<DependencyPropertyInstance>();

        private DependencyPropertyExtractor()
        {

        }

        public static DependencyPropertyExtractor Extract<S>(Expression<Func<S>> expression)
        {
            var visitor = new DependencyPropertyExtractor();
            visitor.Visit(expression);
            return visitor;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            Visit(node.Right);

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Visit(node.Expression);

            var mi = node.Member;
            var declaringType = mi.DeclaringType;

            if (declaringType != null && typeof(DependencyObject).IsAssignableFrom(declaringType))
            {
                var propField = declaringType.GetField(mi.Name + "Property", BindingFlags.Public | BindingFlags.Static);

                if (propField != null)
                {
                    var exp = Expression.Lambda<Func<DependencyObject>>(node.Expression);
                    var func = exp.Compile();
                    var y = func();

                    _extractedItems.Add(new DependencyPropertyInstance
                    {
                        Owner = y,
                        Property = propField.GetValue(y) as DependencyProperty
                    });
                }
            }

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node;
        }
    }
}

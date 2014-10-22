using GenericExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WaitingOnExpressions.Logic
{
    public class TypeExtractor<T> : ExpressionVisitor
    {
        public IReadOnlyCollection<T> ExtractedItems { get { return _extractedItems; } }
        private readonly List<T> _extractedItems = new List<T>();

        private TypeExtractor()
        {

        }

        public static TypeExtractor<T> Extract<S>(Expression<Func<S>> expression)
        {
            var visitor = new TypeExtractor<T>();
            visitor.Visit(expression);
            return visitor;
        }

        private void ExtractFromT(Type t, Expression node)
        {
            if (typeof(T).IsAssignableFrom(t))
            {
                var exp = Expression.Lambda<Func<T>>(node);
                var func = exp.Compile();
                var y = func();

                _extractedItems.Add(y);
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Visit(node.Expression);

            node.Member.If()
                       .Is<FieldInfo>(_ => ExtractFromT(_.FieldType, node))
                       .Is<PropertyInfo>(_ => ExtractFromT(_.PropertyType, node));

            return node; //base.Visit(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            ExtractFromT(node.Value.GetType(), node);
            return node;  //base.Visit(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ExtractFromT(node.Type, node);
            return node; // base.Visit(node);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WaitingOnExpressions.Logic
{
    public class ExpressionAwaitable
    {
        private readonly Func<bool> _predicate;
        private List<INotifyPropertyChanged> _iNotifyPropChangedItems;
        private List<INotifyCollectionChanged> _iNotifyCollectionChangedItems;
        private List<DependencyPropertyExtractor.DependencyPropertyInstance> _iDPItems;
        private ExpressionAwaiter _awaiter = new ExpressionAwaiter();
        private bool _wasFalse;

        public ExpressionAwaitable(Expression<Func<bool>> expression)
        {
            _predicate = expression.Compile();
            if (!_predicate())
            {
                _wasFalse = true;
            }

            _iNotifyPropChangedItems = TypeExtractor<INotifyPropertyChanged>
                .Extract(expression).ExtractedItems.ToList();
            _iNotifyCollectionChangedItems = TypeExtractor<INotifyCollectionChanged>
                .Extract(expression).ExtractedItems.ToList();
            _iDPItems = DependencyPropertyExtractor.Extract(expression).ExtractedItems.ToList();

            HookEvents();
        }

        private void HookEvents()
        {
            foreach (var item in _iNotifyPropChangedItems)
            {
                item.PropertyChanged += NotifyPropChanged;
            }
            foreach (var item in _iNotifyCollectionChangedItems)
            {
                item.CollectionChanged += CollectionChanged; ;
            }
            foreach (var item in _iDPItems)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(item.Property, item.Owner.GetType());
                descriptor.AddValueChanged(item.Owner, DependencyPropertyChanged);
            }
        }

        private void ExpressionChanged()
        {
            var isPredicateTrue = _predicate();
            if (!isPredicateTrue) _wasFalse = true;

            if (isPredicateTrue && _wasFalse)
            {
                UnhookEvents();
                _awaiter.Complete();
            }
        }

        private void UnhookEvents()
        {
            foreach (var item in _iNotifyPropChangedItems)
            {
                item.PropertyChanged -= NotifyPropChanged;
            }
            foreach (var item in _iNotifyCollectionChangedItems)
            {
                item.CollectionChanged -= CollectionChanged;
            }
            foreach (var item in _iDPItems)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(item.Property, item.Owner.GetType());
                descriptor.RemoveValueChanged(item.Owner, DependencyPropertyChanged);
            }
        }

        private void NotifyPropChanged(object sender, PropertyChangedEventArgs agrs)
        {
            ExpressionChanged();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ExpressionChanged();
        }

        private void DependencyPropertyChanged(object sender, EventArgs args)
        {
            ExpressionChanged();
        }

        public ExpressionAwaiter GetAwaiter()
        {
            return _awaiter;
        }
    }

}

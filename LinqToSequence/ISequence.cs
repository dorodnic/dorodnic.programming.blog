using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    public interface ISequence<out T>
    {
        ISequenceIterator<T> Begin();
    }

    public interface ISequenceIterator<out T>
    {
        T Next();
    }

    public interface IOption<out T>
    {
        bool HasValue { get; }
        T Value { get; }
    }

    public class Option<T> : IOption<T>
    {
        public Option()
        {
            HasValue = false;
            Value = default(T);
        }

        public Option(T value)
        {
            HasValue = true;
            Value = value;
        }

        public bool HasValue { get; private set; }
        public T Value { get; private set; }

        public override string ToString()
        {
            if (HasValue) return Value.ToString();
            else return "_";
        }
    }

    public interface ISequenceGrouping<out TKey, out TElement> : ISequence<TElement>
    {
        TKey Key { get; }
    }

    public class SequenceGrouping<TKey, TElement> : ISequenceGrouping<TKey, TElement>
    {
        private readonly TKey _key;
        private readonly ISequence<TElement> _sequnce;

        public SequenceGrouping(TKey key, ISequence<TElement> sequnce)
        {
            _key = key;
            _sequnce = sequnce;
        }

        public TKey Key
        {
	        get { return _key; }
        }

        public ISequenceIterator<TElement> Begin()
        {
            return _sequnce.Begin();
        }

        public override string ToString()
        {
            return Key + ": { " + _sequnce.ToString(15) + "... }";
        }
    }

    public class FunctionalSequence<T> : ISequence<T>
    {
        private readonly Func<Func<T>> _generator;

        private class FunctionalSequenceIterator<T> : ISequenceIterator<T>
        {
            private readonly Func<T> _next;

            public FunctionalSequenceIterator(Func<T> next)
            {
                _next = next;
            }

            public T Next()
            {
                return _next();
            }
        }
        
        public FunctionalSequence(Func<Func<T>> generator)
        {
            _generator = generator;
        }

        public ISequenceIterator<T> Begin()
        {
            return new FunctionalSequenceIterator<T>(_generator());
        }
    }

    public static class SequenceExtensions
    {
        public static IOption<T> FromValue<T>(T value)
        {
            return new Option<T>(value);
        }

        public static IOption<T> Empty<T>()
        {
            return new Option<T>();
        }

        public static string ToString<T>(this ISequence<T> @this, int n)
        {
            var iterator = @this.Begin();
            return string.Join(", ", Enumerable.Range(0, n - 1)
                .Select(x => iterator.Next())
                .Select(x => x.ToString()).ToArray());
        }

        public static List<T> Take<T>(this ISequence<IOption<T>> @this, int n)
        {
            var iterator = @this.Begin();
            return Enumerable.Range(0, n - 1)
                .Select(x => iterator.Next())
                .Where(option => option.HasValue)
                .Select(option => option.Value).ToList();
        }

        //public static List<T> Take<T>(this ISequence<T> @this, int n)
        //{
        //    var iterator = @this.Begin();
        //    return Enumerable.Range(1, n).Select(x => iterator.Next()).ToList();
        //}

public static T At<T>(this ISequence<T> @this, long n)
{
    var iterator = @this.Begin();
    for (long i = 0; i < n; i++)
    {
        iterator.Next();
    }
    return iterator.Next();
}

        //public static T At<T>(this ISequence<T> @this, long n)
        //{
        //    var iterator = @this.Begin();
        //    for (long i = 0; i < n - 1; i++)
        //    {
        //        iterator.Next();
        //    }
        //    return iterator.Next();
        //}

        public static ISequence<IOption<S>> Select<S, T>(this ISequence<IOption<T>> @this, Func<T, S> mapFunc)
        {
            return new FunctionalSequence<IOption<S>>(() =>
            {
                var tIter = @this.Begin();
                return () =>
                {
                    var t = tIter.Next();
                    return t.HasValue ? FromValue(mapFunc(t.Value)) : Empty<S>();
                };
            });
        }

        //public static ISequence<Option<T>> Where<T>(this ISequence<Option<T>> @this, Func<T, bool> filter)
        //{
        //    return @this.Select(t => filter(t) ? new Option<T>(t) : new Option<T>());
        //}

        //public static ISequence<Option<T>> Where<T>(this ISequence<T> @this, Func<T, bool> filter)
        //{
        //    return @this.Select(t => filter(t) ? new Option<T>(t) : new Option<T>());
        //}

        //public static ISequence<S> Select<S, T>(this ISequence<Option<T>> @this, Func<T, S> mapFunc)
        //{
        //    return @this.Where1(t => t.HasValue).Select(o => mapFunc(o.Value));
        //}

        public static ISequence<IOption<T>> Where<T>(this ISequence<IOption<T>> @this, Func<T, bool> filter)
        {
            return new FunctionalSequence<IOption<T>>(() =>
            {
                var tIter = @this.Begin();
                return () =>
                {
                    var t = tIter.Next();
                    return t.HasValue && filter(t.Value) ? t : Empty<T>();
                };
            });
        }

        //public static ISequence<T> Where<T>(this ISequence<T> @this, Func<T, bool> filter)
        //{
        //    return new FunctionalSequence<T>(() =>
        //    {
        //        var tIter = @this.Begin();
        //        return () =>
        //        {
        //            var t = tIter.Next();

        //            while (!filter(t)) t = tIter.Next();

        //            return t;
        //        };
        //    });
        //}

        //public static ISequence<T> Skip<T>(this ISequence<T> @this, int n)
        //{
        //    return new FunctionalSequence<T>(() =>
        //    {
        //        var tIter = @this.Begin();
        //        for (var i = 0; i < n; i++)
        //        {
        //            tIter.Next();
        //        }
        //        return tIter.Next;
        //    });
        //}

        public static ISequence<IOption<T>> Nothing<T>()
        {
            return new FunctionalSequence<IOption<T>>(() => () => Empty<T>());
        }

        //public static ISequence<Option<TResult>> SelectMany<TSource, TCollection, TResult>
        //    (this ISequence<Option<TSource>> source,
        //        Func<TSource, ISequence<TCollection>> collectionSelector,
        //        Func<TSource, TCollection, TResult> resultSelector)
        //{
        //    return
        //        source.SelectMany(option => option.HasValue ? collectionSelector(option.Value).Select(t => new Option<TCollection>(t)) : Nothing<TCollection>(),
        //            (option, cItem) =>
        //                option.HasValue && cItem.HasValue
        //                    ? new Option<TResult>(resultSelector(option.Value, cItem.Value))
        //                    : new Option<TResult>());
        //}

        public static ISequence<IOption<TResult>> SelectMany<TSource, TCollection, TResult>
               (this ISequence<IOption<TSource>> source,
                Func<TSource, ISequence<IOption<TCollection>>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
        {
            return new FunctionalSequence<IOption<TResult>>(() =>
            {
                long g = 0;
                return () =>
                {
                    var pair = NtoN2Mapping.GetPair(g++);
                    var t = source.At(pair.I);

                    if (t.HasValue)
                    {
                        var cs = collectionSelector(t.Value);

                        var c = cs.At(pair.J);

                        return c.HasValue ? FromValue(resultSelector(t.Value, c.Value)) : Empty<TResult>();
                    }
                    else
                    {
                        return Empty<TResult>();
                    }
                };
            });
        }

        //public static ISequence<TResult> SelectMany<TSource, TCollection, TResult>
        //    (this ISequence<TSource> source,
        //     Func<TSource, ISequence<TCollection>> collectionSelector, 
        //     Func<TSource, TCollection, TResult> resultSelector)
        //{
        //    return new FunctionalSequence<TResult>(() =>
        //    {
        //        long g = 0;
        //        return () =>
        //        {
        //            var pair = NtoN2Mapping.GetPair(g++);
        //            var t = source.At(pair.I);
        //            var cs = collectionSelector(t);
        //            var c = cs.At(pair.J);

        //            return resultSelector(t, c);
        //        };
        //    });
        //}

        //public static ISequence<Option<ISequenceGrouping<TKey, Option<TSource>>>> GroupBy<TSource, TKey>(
        //    this ISequence<Option<TSource>> source,
        //    Func<TSource, TKey> keySelector)
        //{
        //    return source.GroupBy(keySelector, t => t);
        //}
        
// Helper function to zip two regular sequences together
public static ISequence<TResult> Zip<TSource, TOther, TResult>(
    this ISequence<TSource> @this,
    ISequence<TOther> other,
    Func<TSource, TOther, TResult> mapFunc)
{
    return new FunctionalSequence<TResult>(() =>
    {
        var tIter = @this.Begin();
        var sIter = other.Begin();
        return () => mapFunc(tIter.Next(), sIter.Next());
    });
}

// The short version of distinct
public static ISequence<IOption<TSource>> Distinct<TSource>(this ISequence<IOption<TSource>> @this)
{
    return @this.Distinct(x => x);
}

// The full version of distinct
public static ISequence<IOption<TSource>> Distinct<TSource, TKey>(
    this ISequence<IOption<TSource>> @this,
    Func<TSource, TKey> keySelector)
{
    return Sequences.NaturalNumbers
                    .Select(x => x + 1) // 1, 2, 3, 4, 5, ...
                    .Zip(@this, (indexOption, itemOption) => FromValue(
                        new
                        {
                            index = indexOption.Value,
                            option = itemOption
                        })) // (s1, 1), (s2, 2), (s3, 3), ...
                    .Where(t =>
                    {
                        if (t.option.HasValue)
                        {
                            var key = keySelector(t.option.Value);
                            // Is t a new value?
                            return @this.Take(t.index).All(option => !key.Equals(keySelector(option)));
                        }
                        else
                        {
                            return false;
                        }
                    })
                    .Select(t => t.option.Value);
}

        //return new FunctionalSequence<Option<TSource>>(() =>
            //{
            //    var iter = @this.Begin();
            //    var idx = 0;

            //    return () =>
            //    {
            //        var option = iter.Next();
            //        idx++;

            //        if (option.HasValue)
            //        {
            //            var key = keySelector(option.Value);

            //            var found = false;

            //            var iter2 = @this.Begin();
            //            for (var i = 1; i < idx; i++)
            //            {
            //                var option2 = iter2.Next();
            //                if (option2.HasValue)
            //                {
            //                    if (keySelector(option.Value).Equals(key))
            //                    {
            //                        found = true;
            //                    }
            //                }
            //            }

            //            return found ? new Option<TSource>() : option;
            //        }
            //        else
            //        {
            //            return option;
            //        }
            //    };
            //});

        public static ISequence<IOption<ISequenceGrouping<TKey, IOption<TElement>>>> GroupBy<TSource, TKey, TElement>(
            this ISequence<IOption<TSource>> @this,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elemSelector)
        {
            return @this.Distinct(keySelector).Select(elem =>
            {
                var key = keySelector(elem);
                return new SequenceGrouping<TKey, IOption<TElement>>(
                    key,
                    from x in @this where keySelector(x).Equals(key) select elemSelector(x));
            });
        }

        // Alternative overload required for translation of LINQ queries
        public static ISequence<IOption<ISequenceGrouping<TKey, IOption<TSource>>>> GroupBy<TSource, TKey>(
            this ISequence<IOption<TSource>> @this,
            Func<TSource, TKey> keySelector)
        {
            return @this.GroupBy(keySelector, x => x);
        }

        //public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        //public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        //public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer);
        //public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector);
        //public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
        //public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer);


        internal class SelectSequence<S, T> : ISequence<S>
        {
            private class SelectSequenceIterator<S, T> : ISequenceIterator<S>
            {
                private ISequenceIterator<T> _originalIterator;
                private readonly Func<T, S> _mapFunc;

                public SelectSequenceIterator(
                    ISequenceIterator<T> originalIterator,
                    Func<T, S> mapFunc)
                {
                    _originalIterator = originalIterator;
                    _mapFunc = mapFunc;
                }

                public S Next()
                {
                    return _mapFunc(_originalIterator.Next());
                }
            }

            private readonly ISequence<T> _original;
            private readonly Func<T, S> _mapFunc;

            public SelectSequence(ISequence<T> original, Func<T, S> mapFunc)
            {
                _original = original;
                _mapFunc = mapFunc;
            }

            public ISequenceIterator<S> Begin()
            {
                return new SelectSequenceIterator<S, T>(_original.Begin(), _mapFunc);
            }
        }
    }
}

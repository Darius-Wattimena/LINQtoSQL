using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace LINQStyleToSQL
{
    static class Queries
    {
        private static Y Then<T, Y>(this T item, Expression<Func<T, Y>> filter) => filter.Compile()(item);

        private static IEnumerable<Y> Map<T, Y>(this QueryableCollection<T> items, Expression<Func<T, Y>> map)
        {
            foreach (var item in items.Data)
            {
                yield return item.Then(map);
            }
        }

        private static IEnumerable<T> Filter<T>(this QueryableCollection<T> items, Expression<Func<T, bool>> filter)
        {
            foreach (var item in items.Data)
            {
                if (item.Then(filter))
                {
                    yield return item;
                }
            }
        }

        private static QueryableCollection<T> ToQueryableCollection<T>(this ICollection<T> objects, Type type)
        {
            return new QueryableCollection<T>(objects, type);
        }

        public static SelectQuery<T, Y> Select<T, Y>(this ICollection<T> collection, Expression<Func<T, Y>> filter)
        {
            return Select(collection.ToQueryableCollection(typeof(Y)), filter);
        }

        public static QueryableCollection<Y> Where<T, Y>(this SelectQuery<T, Y> query,
            Expression<Func<T, bool>> predicate) where T : class, new() where Y : class
        {
            var collection = query.Collection;

            collection.Builder.AddCommand(new SQLCommand { Expression = predicate, Type = SqlCommandType.WHERE });

            return collection.Then(x => x.Builder.Build<T, Y>());
        }

        public static SelectQuery<T, Y> Select<T, Y>(this QueryableCollection<T> collection,
            Expression<Func<T, Y>> select)
        {
            return new SelectQuery<T, Y>(
                collection.Then(x => new QueryableCollection<T>(collection,
                        new SQLCommand {Expression = select, Type = SqlCommandType.SELECT}))
                    .Then(x => new QueryableCollection<T>(collection,
                        new SQLCommand {Expression = select, Type = SqlCommandType.FROM}))
            );
        }

        /*public static IEnumerable<Y> MSelect<T, Y>(this ISequence<T> source, Expression<Func<T, Y>> filter)
        {
            var query = new Queryable<T>
            {
                Expression = filter, 
                Items = source
            };

            foreach (var item in source)
            {
                
            }

            yield break;
        }

        public static IEnumerable<Y> Select<T, Y>(this IEnumerable<T> items, Expression<Func<T, Y>> filter)
        {
            Console.WriteLine(filter);

            //TODO do SQL stuff here

            foreach (var item in items)
            {
                yield return item.Then(filter);
            }
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> items, Expression<Func<T, bool>> filter)
        {
            Console.WriteLine(filter);

            //TODO do SQL stuff here

            foreach (var item in items)
            {
                if (item.Then(filter))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<(T, U)> Include<T, Y, U>(this IEnumerable<T> items, Expression<Func<T, Y>> includeItemsFilter, Expression<Func<Y, U>> filter)
        {
            Console.WriteLine(filter);

            foreach (var item in items)
            {
                var includeItem = item.Then(includeItemsFilter);
                var filteredIncludeItem = includeItem.Then(filter);
                yield return (item, filteredIncludeItem);
            }
        }*/
    }
}

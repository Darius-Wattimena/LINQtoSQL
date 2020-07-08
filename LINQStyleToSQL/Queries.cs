using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace LINQStyleToSQL
{
    static class Queries
    {
        //TODO helper function should be replaced with SQL
        private static Y Then<T, Y>(this T item, Expression<Func<T, Y>> filter) => filter.Compile()(item);

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
            foreach (var item in items)
            {
                var includeItem = item.Then(includeItemsFilter);
                var filteredIncludeItem = includeItem.Then(filter);
                yield return (item, filteredIncludeItem);
            }
        }
    }
}

﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications
{
    internal static class EnumerableExtensions
    {
        public static void ShouldEachConformTo<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var source = new List<T>(list);
            var func = condition.Compile();

            var failingItems = source
                .Where(x => func(x) == false)
                .ToArray();

            if (failingItems.Any())
            {
                throw new SpecificationException(string.Format(@"Should contain only elements conforming to: {0}" + Environment.NewLine + "the following items did not meet the condition: {1}",
                    condition,
                    failingItems.EachToUsefulString()));
            }
        }

        public static void ShouldContain(this IEnumerable list, params object[] items)
        {
            var actualList = list.Cast<object>();
            var expectedList = items.Cast<object>();

            actualList.ShouldContain(expectedList);
        }

        public static void ShouldContain<T>(this IEnumerable<T> list, params T[] items)
        {
            list.ShouldContain((IEnumerable<T>) items);
        }

        public static void ShouldContain<T>(this IEnumerable<T> list, IEnumerable<T> items)
        {
            var comparer = new AssertComparer<T>();

            var listArray = list.ToArray();
            var itemsArray = items.ToArray();

            var noContain = itemsArray
                .Where(x => !listArray.Contains(x, comparer))
                .ToList();

            if (noContain.Any())
            {
                throw new SpecificationException(string.Format(
                    @"Should contain: {0}" + Environment.NewLine +
                    "entire list: {1}" + Environment.NewLine +
                    "does not contain: {2}",
                    itemsArray.EachToUsefulString(),
                    listArray.EachToUsefulString(),
                    noContain.EachToUsefulString()));
            }
        }

        public static void ShouldContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();
            var listArray = list.ToArray();

            if (!listArray.Any(func))
            {
                throw new SpecificationException(string.Format(
                    @"Should contain elements conforming to: {0}" + Environment.NewLine +
                    "entire list: {1}",
                    condition,
                    listArray.EachToUsefulString()));
            }
        }

        public static void ShouldNotContain(this IEnumerable list, params object[] items)
        {
            var actualList = list.Cast<object>();
            var expectedList = items.Cast<object>();

            actualList.ShouldNotContain(expectedList);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, params T[] items)
        {
            list.ShouldNotContain((IEnumerable<T>) items);
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, IEnumerable<T> items)
        {
            var comparer = new AssertComparer<T>();

            var listArray = list.ToArray();
            var itemsArray = items.ToArray();

            var contains = itemsArray
                .Where(x => listArray.Contains(x, comparer))
                .ToList();

            if (contains.Any())
            {
                throw new SpecificationException(string.Format(
                    @"Should not contain: {0}" + Environment.NewLine +
                    "entire list: {1}" + Environment.NewLine +
                    "does contain: {2}",
                    itemsArray.EachToUsefulString(),
                    listArray.EachToUsefulString(),
                    contains.EachToUsefulString()));
            }
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> list, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();

            var listArray = list.ToArray();
            var contains = listArray.Where(func).ToArray();

            if (contains.Any())
            {
                throw new SpecificationException(string.Format(
                    @"No elements should conform to: {0}" + Environment.NewLine +
                    "entire list: {1}" + Environment.NewLine +
                    "does contain: {2}",
                    condition,
                    listArray.EachToUsefulString(),
                    contains.EachToUsefulString()));
            }
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            var items = collection.Cast<object>().ToArray();

            if (items.Any())
            {
                throw NewException("Should be empty but contains:\n" + items.EachToUsefulString());
            }
        }

        public static void ShouldNotBeEmpty(this IEnumerable collection)
        {
            if (!collection.Cast<object>().Any())
            {
                throw NewException("Should not be empty but is");
            }
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> list, params T[] items)
        {
            list.ShouldContainOnly((IEnumerable<T>) items);
        }

        public static void ShouldContainOnly<T>(this IEnumerable<T> list, IEnumerable<T> items)
        {
            var listArray = list.ToArray();
            var itemsArray = items.ToArray();

            var source = new List<T>(listArray);
            var noContain = new List<T>();
            var comparer = new AssertComparer<T>();

            foreach (var item in itemsArray)
            {
                if (!source.Contains(item, comparer))
                {
                    noContain.Add(item);
                }
                else
                {
                    source.Remove(item);
                }
            }

            if (noContain.Any() || source.Any())
            {
                var message = string.Format(@"Should contain only: {0}" + Environment.NewLine + "entire list: {1}",
                    itemsArray.EachToUsefulString(),
                    listArray.EachToUsefulString());

                if (noContain.Any())
                {
                    message += "\ndoes not contain: " + noContain.EachToUsefulString();
                }

                if (source.Any())
                {
                    message += "\ndoes contain but shouldn't: " + source.EachToUsefulString();
                }

                throw new SpecificationException(message);
            }
        }
    }
}

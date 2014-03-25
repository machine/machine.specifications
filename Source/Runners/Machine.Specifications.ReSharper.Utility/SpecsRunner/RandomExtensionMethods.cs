namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class RandomExtensionMethods
    {
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable)
            {
                action(t);
            }
        }

        internal static void InvokeAll(this IEnumerable<Delegate> contextActions, params object[] args)
        {
            contextActions.AllNonNull().Select<Delegate, Action>(x => () => x.DynamicInvoke(args)).InvokeAll();
        }

        static IEnumerable<T> AllNonNull<T>(this IEnumerable<T> elements) where T : class
        {
            return elements.Where(x => x != null);
        }

        static void InvokeAll(this IEnumerable<Action> actions)
        {
            actions.Each(x => x());
        }

        internal static bool HasAttribute<TAttribute>(this ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider.GetCustomAttributes(typeof(TAttribute), true).Any();
        }
    }
}
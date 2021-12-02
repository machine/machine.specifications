using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Sdk;

namespace Machine.Specifications.Utility
{
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
            contextActions.AllNonNull().Select<Delegate, Action>(x => () => x.InvokeAsync(args)).InvokeAll();
        }

        private static void InvokeAsync(this Delegate target, params object[] args)
        {
            var runner = new DelegateRunner(target, args);
            runner.Execute();
        }

        private static IEnumerable<T> AllNonNull<T>(this IEnumerable<T> elements) where T : class
        {
            return elements.Where(x => x != null);
        }

        private static void InvokeAll(this IEnumerable<Action> actions)
        {
            actions.Each(x => x());
        }

        internal static bool HasAttribute<TAttributeFullName>(this MemberInfo attributeProvider, TAttributeFullName attributeFullName)
            where TAttributeFullName : AttributeFullName
        {
            var attributeType = Type.GetType(attributeFullName.FullName);

            if (attributeType == null)
            {
                return false;
            }

            return attributeProvider.GetCustomAttributes(attributeType, true).Any();
        }

        internal static AttributeFullName GetCustomDelegateAttributeFullName(this Type type)
        {
            // TODO: Make smarter
            var attributeType = Type.GetType(new ActDelegateAttributeFullName());
            var typeInfo = type.GetTypeInfo();

            if (attributeType != null && typeInfo.GetCustomAttributes(attributeType, false).Any())
            {
                return new ActDelegateAttributeFullName();
            }

            attributeType = Type.GetType(new AssertDelegateAttributeFullName());

            if (attributeType != null && typeInfo.GetCustomAttributes(attributeType, false).Any())
            {
                return new AssertDelegateAttributeFullName();
            }

            attributeType = Type.GetType(new BehaviorDelegateAttributeFullName());

            if (attributeType != null && typeInfo.GetCustomAttributes(attributeType, false).Any())
            {
                return new BehaviorDelegateAttributeFullName();
            }

            attributeType = Type.GetType(new CleanupDelegateAttributeFullName());

            if (attributeType != null && typeInfo.GetCustomAttributes(attributeType, false).Any())
            {
                return new CleanupDelegateAttributeFullName();
            }

            attributeType = Type.GetType(new SetupDelegateAttributeFullName());

            if (attributeType != null && typeInfo.GetCustomAttributes(attributeType, false).Any())
            {
                return new SetupDelegateAttributeFullName();
            }

            return null;
        }
    }
}

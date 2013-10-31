using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Machine.Specifications.Sdk
{
    public static class SpecTypeCheck
    {

        public static bool IsSpecClass(ITypeInfo type)
        {
            return IsContext(type);
        }

        public static bool IsContext(ITypeInfo type)
        {
            return (!type.IsAbstract &&
                !type.IsStruct &&
                type.GenericParametersCount == 0 &&
                !type.HasBehaviorAttributeName &&
                (type.ExistsAnySpecifications() || type.ExistsAnyBehaviors()));
        }
    }
}

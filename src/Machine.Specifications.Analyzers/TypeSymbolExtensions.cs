using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Machine.Specifications.Analyzers;

internal static class TypeSymbolExtensions
{
    public static bool Extends(this ITypeSymbol? symbol, Type? type)
    {
        if (symbol == null || type == null)
        {
            return false;
        }

        while (symbol != null)
        {
            if (symbol.Matches(type))
            {
                return true;
            }

            symbol = symbol.BaseType;
        }

        return false;
    }

    public static bool Matches(this ITypeSymbol symbol, Type type)
    {
        switch (symbol.SpecialType)
        {
            case SpecialType.System_Void:
                return type == typeof(void);

            case SpecialType.System_Boolean:
                return type == typeof(bool);

            case SpecialType.System_Int32:
                return type == typeof(int);

            case SpecialType.System_Single:
                return type == typeof(float);
        }

        if (type.IsArray)
        {
            return symbol is IArrayTypeSymbol array && Matches(array.ElementType, type.GetElementType()!);
        }

        if (symbol is not INamedTypeSymbol named)
        {
            return false;
        }

        if (type.IsConstructedGenericType)
        {
            var args = type.GetTypeInfo().GenericTypeArguments;

            if (args.Length != named.TypeArguments.Length)
            {
                return false;
            }

            for (var i = 0; i < args.Length; i++)
            {
                if (!Matches(named.TypeArguments[i], args[i]))
                {
                    return false;
                }
            }

            return Matches(named.ConstructedFrom, type.GetGenericTypeDefinition());
        }

        return named.Name == type.Name && named.ContainingNamespace?.ToDisplayString() == type.Namespace;
    }
}

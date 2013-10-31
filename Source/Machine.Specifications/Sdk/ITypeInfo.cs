using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Machine.Specifications.Sdk
{
    /// <summary>
    /// Represents information about a type.
    /// </summary>
    public interface ITypeInfo
    {
        /// <summary>
        /// Gets a value indicating the paramet count of the type
        /// </summary>
        int GenericParametersCount { get; }

        /// <summary>
        /// Gets a value indicating wheter the type has a Behavior Attribute
        /// </summary>
        bool HasBehaviorAttributeName { get;  }

        /// <summary>
        /// Gets a value indicating whether the type is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether the type is sealed.
        /// </summary>
        bool IsSealed { get; }

        /// <summary>
        /// Determines if the type is a struct
        /// </summary>
        bool IsStruct { get; }

        /// <summary>
        /// Determines if field is of type It
        /// </summary>
        /// <returns></returns>
        bool ExistsAnySpecifications();

        /// <summary>
        /// Determines if field is has Behaviors Attribute or is a type of Behaves_like
        /// </summary>
        /// <returns></returns>
        bool ExistsAnyBehaviors();

        /// <summary>
        /// Gets the subject string of the Subject Attribute
        /// </summary>
        /// <returns></returns>
        string GetStringOfSubjectAttribute();
    }
}
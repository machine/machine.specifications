//-------------------------------------------------------------------------------
// <copyright file="SafeDereferenceExtensionMethods.cs" company="bbv Software Services AG">
//   Copyright (c) 2013
//   
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//   http://www.apache.org/licenses/LICENSE-2.0
//   
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Machine.Specifications.ReSharperRunner
{
    using System;

    public static class SafeDereferenceExtensionMethods
    {
        public static TResult SafelyDereference<T, TResult>(this T container, Func<T, TResult> dereference) where T : class where TResult : class
        {
            return container != null ? dereference(container) : null;
        }

        public static TResult? SafelyDereferenceToNullable<T, TResult>(this T container, Func<T, TResult> dereference)
            where T : class
            where TResult : struct
        {
            return container != null ? dereference(container) : (TResult?)null;
        }

        public static TResult SafelyDereference<T, TResult>(this T? container, Func<T?, TResult> dereference)
            where T : struct
            where TResult : class
        {
            return container.HasValue ? dereference(container) : null;
        }
    }
}
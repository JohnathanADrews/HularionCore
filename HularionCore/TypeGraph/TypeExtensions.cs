#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HularionCore.TypeGraph
{
    /// <summary>
    /// Adds some useful extensions to Type.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns true iff the property has both a getter and setter and they are static.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>true iff the property has both a getter and setter and they are static.</returns>
        public static bool IsStatic(this PropertyInfo property)
        {
            return ((property.GetMethod != null && property.GetMethod.IsStatic) || (property.SetMethod != null && property.SetMethod.IsStatic));
        }

        /// <summary>
        /// Gets the non-static properties with getters and setters.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>The non-static properties with getters and setters.</returns>
        public static IEnumerable<PropertyInfo> GetNonStaticGetSetProperties(this Type type)
        {
            if (type.IsClass) { return type.GetProperties().Where(x => !x.IsStatic() && x.GetMethod != null && x.SetMethod != null).ToList(); }
            //structs
            return type.GetProperties().Where(x => !x.IsStatic()).ToList();
        }

        /// <summary>
        /// Returns true iff the field is static.
        /// </summary>
        /// <param name="field">The field to check.</param>
        /// <returns>true iff the field is static.</returns>
        public static bool IsStatic(this FieldInfo field)
        {
            return field.IsStatic;
        }

        /// <summary>
        /// Gets the non-static fields.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>The non-static fields.</returns>
        public static IEnumerable<FieldInfo> GetNonStaticFields(this Type type)
        {
            return type.GetFields().Where(x => !x.IsStatic()).ToList();
        }


    }
}

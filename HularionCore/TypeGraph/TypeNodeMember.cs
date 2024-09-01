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
using System.Reflection;
using System.Text;

namespace HularionCore.TypeGraph
{
    /// <summary>
    /// Represents the member (property/field) of a type.
    /// </summary>
    public class TypeNodeMember
    {
        /// <summary>
        /// The node to which this member belongs.
        /// </summary>
        public TypeNode Parent { get; set; }

        /// <summary>
        /// The name of the member.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the member.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// True if the type of this member is given by a generic.
        /// </summary>
        public bool IsGenericParameter { get; set; } = false;

        /// <summary>
        /// The value of the member.
        /// </summary>
        public TypeNode Member { get; set; }

        /// <summary>
        /// The property this member represents if it is a property node.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// The field this member represents if it is a field node.
        /// </summary>
        public FieldInfo Field { get; set; }

        /// <summary>
        /// True iff the property is not null.
        /// </summary>
        public bool IsProperty { get { return Property != null; } }

        /// <summary>
        /// True iff the field is not null.
        /// </summary>
        public bool IsField { get { return Field != null; } }

        /// <summary>
        /// Gets the value of the member given the instance of the parent.
        /// </summary>
        /// <param name="parent">The instance from which to extrace the member.</param>
        /// <returns>The value of the member value from the parent instance.</returns>
        public object GetMemberValue(object parent)
        {
            if (IsField)
            {
                return Field.GetValue(parent);
            }
            if (IsProperty)
            {
                if (Property.GetIndexParameters().Count() > 0)
                {
                    return null;
                }
                return Property.GetValue(parent);
            }
            return null;
        }

        public void SetMemberValue(object parent, object value)
        {
            if (IsField)
            {
                Field.SetValue(parent, value);
            }
            if (IsProperty)
            {
                if (Property.GetIndexParameters().Count() > 0)
                {
                    return;
                }
                Property.SetValue(parent, value);
            }
        }

    }
}

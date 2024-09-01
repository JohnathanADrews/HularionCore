#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.Pattern.Functional;
using HularionCore.Structure;
using HularionCore.TypeGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionCore.General
{
    /// <summary>
    /// Maps the member values of one type to another type by member name.
    /// </summary>
    public class MemberMapper
    {

        /// <summary>
        /// Maps "from" the properties to the "to" members.
        /// </summary>
        private Dictionary<PropertyInfo, PropertyInfo> propertyMap = new Dictionary<PropertyInfo, PropertyInfo>();

        /// <summary>
        /// Contains the nullable properties.
        /// </summary>
        private Dictionary<PropertyInfo, bool> nullableProperties = new Dictionary<PropertyInfo, bool>();

        /// <summary>
        /// Maps "from" the properties to the "to" members.
        /// </summary>
        private Dictionary<FieldInfo, FieldInfo> fieldMap = new Dictionary<FieldInfo, FieldInfo>();

        /// <summary>
        /// Contains the nullable properties.
        /// </summary>
        private Dictionary<FieldInfo, bool> nullableFields = new Dictionary<FieldInfo, bool>();

        //Copies null values over existing values if true.
        private bool includeNulls;

        /// <summary>
        /// The type beng copied from.
        /// </summary>
        public Type FromType { get; private set; }

        /// <summary>
        /// The type being copied to.
        /// </summary>
        public Type ToType { get; private set; }

        /// <summary>
        /// A table that maps transform for types.
        /// </summary>
        private Table<Type, Type> transformTable = new Table<Type, Type>();

        /// <summary>
        /// Creates a map from fromType to toType objects.
        /// </summary>
        /// <typeparam name="FromType">The type to map from.</typeparam>
        /// <typeparam name="ToType">The type to map to.</typeparam>
        /// <param name="includeNulls">Copies null values over existing values if true.</param>
        public void CreateMap<FromType, ToType>(bool includeNulls = true)
            where FromType : class
            where ToType : class
        {
            CreateMap(typeof(FromType), typeof(ToType), includeNulls);
        }

        /// <summary>
        /// Creates a map from fromType to toType objects.
        /// </summary>
        /// <param name="fromType">The type to map from.</param>
        /// <param name="toType">The type to map to.</param>
        /// <param name="includeNulls">Copies null values over existing values if true.</param>
        public void CreateMap(Type fromType, Type toType, bool includeNulls = true)
        {
            this.FromType = fromType;
            this.ToType = toType;
            this.includeNulls = includeNulls;

            var fromProperties = fromType.GetNonStaticGetSetProperties();
            var toProperties = toType.GetNonStaticGetSetProperties();

            foreach (var toProperty in toProperties)
            {
                var fromProperty = fromProperties.Where(x => x.Name == toProperty.Name).FirstOrDefault();
                if (fromProperty != null)
                {
                    propertyMap.Add(fromProperty, toProperty);
                }
            }

            foreach (var fromProperty in fromProperties)
            {
                if (!fromProperty.PropertyType.IsValueType || Nullable.GetUnderlyingType(fromProperty.PropertyType) != null)
                {
                    nullableProperties.Add(fromProperty, true);
                }
                else
                {
                    nullableProperties.Add(fromProperty, false);
                }
            }


            var fromFields = fromType.GetNonStaticFields();
            var toFields = toType.GetNonStaticFields();

            foreach (var toField in toFields)
            {
                var fromField = fromFields.Where(x => x.Name == toField.Name).FirstOrDefault();
                if (fromField != null)
                {
                    fieldMap.Add(fromField, toField);
                }
            }

            foreach (var fromField in fromFields)
            {
                if (!fromField.FieldType.IsValueType || Nullable.GetUnderlyingType(fromField.FieldType) != null)
                {
                    nullableFields.Add(fromField, true);
                }
                else
                {
                    nullableFields.Add(fromField, false);
                }
            }


        }

        /// <summary>
        /// Excludes the names provided from being mapped.
        /// </summary>
        /// <param name="memberNames">The names of the members to exclude.</param>
        public void ExcludeMembers(params string[] memberNames)
        {
            foreach (var name in memberNames)
            {
                var key = propertyMap.Keys.Where(x => x.Name == name).FirstOrDefault();
                if (key != null)
                {
                    propertyMap.Remove(key);
                }
            }
            foreach (var name in memberNames)
            {
                var key = fieldMap.Keys.Where(x => x.Name == name).FirstOrDefault();
                if (key != null)
                {
                    fieldMap.Remove(key);
                }
            }
        }

        /// <summary>
        /// Maps the values of "from" to "to"
        /// </summary>
        /// <param name="from">The object to map values from.</param>
        /// <param name="to">The object to map values to.</param>
        public void Map(object from, object to)
        {
            if (from == null || to == null)
            {
                return;
            }
            object value;
            foreach (var item in propertyMap)
            {
                value = null;
                var fromProperty = item.Key;
                var toProperty = item.Value;
                if (transformTable.ContainsColumn(fromProperty.PropertyType) && transformTable.ContainsRow(toProperty.PropertyType) && transformTable.GetValue<Func<object, object>>(fromProperty.PropertyType, toProperty.PropertyType) != null)
                {
                    value = transformTable.GetValue<Func<object, object>>(fromProperty.PropertyType, toProperty.PropertyType)(fromProperty.GetValue(from));
                }
                else
                {
                    value = fromProperty.GetValue(from);
                }
                //Check whether null values should be copied and whether the value is nullable.
                if (!nullableProperties[fromProperty] || includeNulls || value != null)
                {
                    toProperty.SetValue(to, value);
                }
            }
            foreach (var item in fieldMap)
            {
                value = null;
                var fromField = item.Key;
                var toField = item.Value;
                if (transformTable.ContainsColumn(fromField.FieldType) && transformTable.ContainsRow(toField.FieldType) && transformTable.GetValue<Func<object, object>>(fromField.FieldType, toField.FieldType) != null)
                {
                    value = transformTable.GetValue<Func<object, object>>(fromField.FieldType, toField.FieldType)(fromField.GetValue(from));
                }
                else
                {
                    value = fromField.GetValue(from);
                }
                //Check whether null values should be copied and whether the value is nullable.
                if (!nullableFields[fromField] || includeNulls || value != null)
                {
                    toField.SetValue(to, value);
                }
            }
        }

        /// <summary>
        /// Sets a transformer from one type to another type.
        /// </summary>
        /// <typeparam name="S">The type to transform from.</typeparam>
        /// <typeparam name="T">The type to transform to.</typeparam>
        /// <param name="transform">THe transformer.</param>
        public void SetTypeTransform<S, T>(ITransform<S, T> transform)
        {
            this.transformTable.SetValue(typeof(S), typeof(T), new Func<object, object>(input=>transform.Transform((S)input)));
        }
    }
}

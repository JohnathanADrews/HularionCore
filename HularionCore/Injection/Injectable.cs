#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.TypeGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HularionCore.Injector
{
    /// <summary>
    /// Encapsulates the injection process.
    /// </summary>
    /// <typeparam name="InjectableType">The derived type or an interface implemented by the derived type.</typeparam>
    public abstract class Injectable<InjectableType>
    {
        private PropertyInfo[] injectableProperties = typeof(InjectableType).GetNonStaticGetSetProperties().ToArray();
        private FieldInfo[] injectableFields = typeof(InjectableType).GetNonStaticFields().ToArray();

        private Dictionary<string, PropertyInfo> thisProperties;
        private Dictionary<string, FieldInfo> thisFields;

        private Dictionary<InjectorOverwriteMode, Action<InjectableType>> processors = new Dictionary<InjectorOverwriteMode, Action<InjectableType>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public Injectable()
        {
            var thisType = this.GetType();

            thisProperties = thisType.GetProperties().ToDictionary(x => x.Name, x => x);
            thisFields = thisType.GetFields().ToDictionary(x => x.Name, x => x);

            var spinner = new Action<InjectableType, Func<object, object, bool>>((overwriter, determiner) =>
            {
                foreach (var property in injectableProperties)
                {
                    var oValue = property.GetValue(overwriter);
                    var tValue = property.GetValue(this);
                    if (determiner(tValue, oValue)) { thisProperties[property.Name].SetValue(this, oValue); }
                }
                foreach (var field in injectableFields)
                {
                    var oValue = field.GetValue(overwriter);
                    var tValue = field.GetValue(this);
                    if (determiner(tValue, oValue)) { thisFields[field.Name].SetValue(this, oValue); }
                }
            });

            processors.Add(InjectorOverwriteMode.AnyOverAny, overwriter => { spinner(overwriter, (tValue, oValue) => true); });
            processors.Add(InjectorOverwriteMode.AnyOverValue, overwriter => { spinner(overwriter, (tValue, oValue) => (tValue != null)); });
            processors.Add(InjectorOverwriteMode.ValueOverValue, overwriter => { spinner(overwriter, (tValue, oValue) => (tValue != null && oValue != null)); });
            processors.Add(InjectorOverwriteMode.ValueOverNull, overwriter => { spinner(overwriter, (tValue, oValue) => (tValue == null)); });
            processors.Add(InjectorOverwriteMode.NullOverValue, overwriter => { spinner(overwriter, (tValue, oValue) => (tValue != null && oValue == null)); });
        }

        /// <summary>
        /// Sets the members of the derived type instance with the provided values in order, using the provided mode.
        /// </summary>
        /// <param name="mode">The injection mode.</param>
        /// <param name="values">The values to overwrite in this value.</param>
        public void SetMembers(InjectorOverwriteMode mode, params InjectableType[] values)
        {
            var processor = processors[mode];

            for(var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                processor(value);
            }
        }
    }
}

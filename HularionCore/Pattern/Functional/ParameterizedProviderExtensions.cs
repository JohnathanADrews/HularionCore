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
using System.Text;
using System.Threading.Tasks;

namespace HularionCore.Pattern.Functional
{
    /// <summary>
    /// Provides extensions for creating parameterized providers.
    /// </summary>
    public static class ParameterizedProviderExtensions
    {

        /// <summary>
        /// Extends IEnumerable so that a parameterized provider may be creaded from it.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the provider parameter.</typeparam>
        /// <typeparam name="ProvidedType">The type of object the provider provides.</typeparam>
        /// <param name="enumerable">The IEnumerable</param>
        /// <param name="predicate">Determines how to match a parameter to a provided value.</param>
        /// <param name="processesInParallel">Iff true, the provided values will be extracted in parallel.</param>
        /// <returns>An IParameterizedProvider.</returns>
        public static IParameterizedProvider<ParameterType, ProvidedType> MakeParameterizedProvider<ParameterType, ProvidedType>(this IEnumerable<ProvidedType> enumerable, Func<ParameterType, ProvidedType, bool> predicate, bool processesInParallel = false)
        {
            var provider = new ParameterizedProviderFunction<ParameterType, ProvidedType>(x => enumerable.Where(y => predicate(x, y)).FirstOrDefault(), processesInParallel: processesInParallel);
            return provider;
        }

        /// <summary>
        /// Extends IDictionary to create a IParameterizedProvider.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the provider parameter and key type of the dictionary.</typeparam>
        /// <typeparam name="ProvidedType">The type of object the provider provides and value type of the dictionary.</typeparam>
        /// <param name="dictionary">The dictionary acting as the provider.</param>
        /// <returns>A IParameterizedProvider.</returns>
        public static IParameterizedProvider<ParameterType, ProvidedType> MakeParameterizedProvider<ParameterType, ProvidedType>(this IDictionary<ParameterType, ProvidedType> dictionary)
        {
            var result = ParameterizedProvider.FromSingle<ParameterType, ProvidedType>(key =>
            {
                if (dictionary.ContainsKey(key)) { return dictionary[key]; }
                return default(ProvidedType);
            });
            return result;
        }
    }
}

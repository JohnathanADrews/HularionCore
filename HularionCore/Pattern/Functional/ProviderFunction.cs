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
    /// Creates a provider from a Func
    /// </summary>
    /// <typeparam name="ProvidedType">The type to provide.</typeparam>
    public class ProviderFunction<ProvidedType> : IProvider<ProvidedType>
    {

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get { return processor.ProcessPrefixes; } }

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get { return processor.ProcessSuffixes; } }

        private ParameterizedFacadeFunction<bool, ProvidedType> processor;

        /// <summary>
        /// Constructor,
        /// </summary>
        /// <param name="provider">The function that will provide the object of type ProvidedType.</param>
        /// <param name="processesInParallel">If true, the provder is called in parallel when multiple values are provided.</param>
        public ProviderFunction(Func<ProvidedType> provider, bool processesInParallel = false)
        {
            this.processor = new ParameterizedFacadeFunction<bool, ProvidedType>(x =>
            {
                return provider();
            }, processesInParallel: processesInParallel);
        }

        /// <summary>
        /// Constructor,
        /// </summary>
        /// <param name="provider">The function that will provide the object of type ProvidedType.</param>
        public ProviderFunction(Func<IEnumerable<ProvidedType>> provider)
        {
            this.processor = new ParameterizedFacadeFunction<bool, ProvidedType>(new Func<IEnumerable<bool>, IEnumerable<ProvidedType>>(x =>
            {
                return provider();
            }));
        }

        /// <summary>
        /// Provides an object of type T.
        /// </summary>
        /// <returns>An object of type T.</returns>
        public ProvidedType Provide()
        {
            return this.processor.Process(true);
        }

        /// <summary>
        /// Provides values of the given Type.
        /// </summary>
        /// <param name="count">The number of values to return.</param>
        /// <returns>A number of values indicated by count.</returns>
        public IEnumerable<ProvidedType> Provide(int count)
        {
            return this.processor.Process(Enumerable.Repeat<bool>(true, count));
        }
    }
}

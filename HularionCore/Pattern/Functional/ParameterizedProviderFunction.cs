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
    /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
    /// <typeparam name="ProvidedType">The type that is provided.</typeparam>
    public class ParameterizedProviderFunction<ParameterType, ProvidedType> : IParameterizedProvider<ParameterType, ProvidedType>
    {
        private ParameterizedFacadeFunction<ParameterType, ProvidedType> processor;

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get { return processor.ProcessPrefixes; } }

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get { return processor.ProcessSuffixes; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">A function that provides an object of ProvidedType.</param>
        /// <param name="processesInParallel">If true, the provder is called in parallel when multiple values are provided.</param>
        public ParameterizedProviderFunction(Func<ParameterType, ProvidedType> provider, bool processesInParallel = false)
        {
            this.processor = new ParameterizedFacadeFunction<ParameterType, ProvidedType>(provider, processesInParallel: processesInParallel);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">A function that provides objects of ProvidedType.</param>
        public ParameterizedProviderFunction(Func<IEnumerable<ParameterType>, IEnumerable<ProvidedType>> provider)
        {
            this.processor = new ParameterizedFacadeFunction<ParameterType, ProvidedType>(provider);
        }

        /// <summary>
        /// Provides an object of type ProvidedType.
        /// </summary>
        /// <param name="parameter">The parameter to pass to get the provided object/</param>
        /// <returns>An object of type ProvidedType.</returns>
        public ProvidedType Provide(ParameterType parameter)
        {
            return this.processor.Process(parameter);
        }

        /// <summary>
        /// Provides the values corresponding to the provided parameters.
        /// </summary>
        /// <param name="parameters">The parameters indicating which values to provide.</param>
        /// <returns>The values indicated by the parameters.</returns>
        public IEnumerable<ProvidedType> Provide(IEnumerable<ParameterType> parameters)
        {
            return this.processor.Process(parameters);
        }

        /// <summary>
        /// Adds a parameter transform to a processor at the beginning of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a parameter before the process begins.</param>
        public void AddParameterTransformPrefix(ITransform<ParameterType, ParameterType> transform)
        {
            this.processor.AddParameterTransformPrefix(transform);
        }

        /// <summary>
        /// Adds a parameter transform to a processor at the end of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a parameter before the process begins.</param>
        public void AddParameterTransformSuffix(ITransform<ParameterType, ParameterType> transform)
        {
            this.processor.AddParameterTransformSuffix(transform);
        }

        /// <summary>
        /// Clears all of the parameter transforms.
        /// </summary>
        public void ClearParameterTransforms()
        {
            this.processor.ClearParameterTransforms();
        }

        /// <summary>
        /// Adds a parameter transform to a processor a the beginning of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a result after the process ends.</param>
        public void AddResultTransformPrefix(ITransform<ProvidedType, ProvidedType> transform)
        {
            this.processor.AddResultTransformPrefix(transform);
        }

        /// <summary>
        /// Adds a parameter transform to a processor a the end of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a result after the process ends.</param>
        public void AddResultTransformSuffix(ITransform<ProvidedType, ProvidedType> transform)
        {
            this.processor.AddResultTransformSuffix(transform);
        }

        /// <summary>
        /// Clears all of the parameter transforms.
        /// </summary>
        public void ClearResultTransforms()
        {
            this.processor.ClearResultTransforms();
        }
    }


    /// <summary>
    /// Creates parameterized providers using explicit parameters.
    /// </summary>
    public static class ParameterizedProvider
    {
        /// <summary>
        /// Creates a parameterized provider that explicitly handles a single input at a time.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="ProvidedType">The type that is provided.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <param name="processesInParallel">If true, the provder is called in parallel when multiple values are provided.</param>
        /// <returns>A new parameterized provider.</returns>
        public static IParameterizedProvider<ParameterType, ProvidedType> FromSingle<ParameterType, ProvidedType>(Func<ParameterType, ProvidedType> func, bool processesInParallel = false)
        {
            return new ParameterizedProviderFunction<ParameterType, ProvidedType>(func, processesInParallel);
        }

        /// <summary>
        /// Creates a parameterized provider explicitly from an enumerable.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="ProvidedType">The type that is provided.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <returns>A new parameterized provider.</returns>
        public static IParameterizedProvider<ParameterType, ProvidedType> FromEnumerable<ParameterType, ProvidedType>(Func<IEnumerable<ParameterType>, IEnumerable<ProvidedType>> func)
        {
            return new ParameterizedProviderFunction<ParameterType, ProvidedType>(func);
        }
    }
}

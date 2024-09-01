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
    /// A creator from a Func.
    /// </summary>
    /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
    /// <typeparam name="CreatedType">The type to create.</typeparam>
    public class ParameterizedCreatorFunction<ParameterType, CreatedType>
        : IParameterizedCreator<ParameterType, CreatedType>
    {

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get { return facade.ProcessPrefixes; } }

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get { return facade.ProcessSuffixes; } }

        private ParameterizedFacadeFunction<ParameterType, CreatedType> facade;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="singleCreator">The function that will create the instance of T.</param>
        /// <param name="createInParallel">If true, the objects will be created in parallel.</param>
        public ParameterizedCreatorFunction(Func<ParameterType, CreatedType> singleCreator,
            bool createInParallel = false)
        {
            facade = ParameterizedFacade.FromSingle<ParameterType, CreatedType>(parameter =>
            {
                return singleCreator(parameter);
            }, processesInParallel: createInParallel);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="multipleCreator">The function that will create the instance of T.</param>
        public ParameterizedCreatorFunction(Func<IEnumerable<ParameterType>, IEnumerable<CreatedType>> multipleCreator)
        {
            facade = ParameterizedFacade.FromEnumerable<ParameterType, CreatedType>(parameters =>
            {
                return multipleCreator(parameters);
            });
        }

        /// <summary>
        /// Creates an instance of T.
        /// </summary>
        /// <returns>The new instance of T.</returns>
        public CreatedType Create(ParameterType parameter)
        {
            return facade.Process(parameter);
        }

        /// <summary>
        /// Creates instances of type CreatedType.
        /// </summary>
        /// <param name="parameters">The parameters used in creation.</param>
        /// <returns>A new instance of CreatedType for each parameter provided.</returns>
        public IEnumerable<CreatedType> Create(IEnumerable<ParameterType> parameters)
        {
            return facade.Process(parameters);
        }

        /// <summary>
        /// Creates instances of type CreatedType.
        /// </summary>
        /// <param name="parameter">The parameter used in creation of all created objects.</param>
        /// <param name="count">The number of objects to create.</param>
        /// <returns>The new instances of type CreatedType.</returns>
        public IEnumerable<CreatedType> Create(ParameterType parameter, int count)
        {
            var parameters = new List<ParameterType>();
            for(int i = 0; i < count; i++) { parameters.Add(parameter); }
            return facade.Process(parameters);
        }
    }

    /// <summary>
    /// Creates parameterized creators using explicit parameters.
    /// </summary>
    public static class ParameterizedCreator
    {
        /// <summary>
        /// Creates a parameterized creator that explicitly handles a single input at a time.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="CreatedType">The type that is created.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <param name="processesInParallel">If true, the creator is called in parallel when multiple values are created.</param>
        /// <returns>A new parameterized creator.</returns>
        public static IParameterizedCreator<ParameterType, CreatedType> FromSingle<ParameterType, CreatedType>(Func<ParameterType, CreatedType> func, bool processesInParallel = false)
        {
            return new ParameterizedCreatorFunction<ParameterType, CreatedType>(func, processesInParallel);
        }

        /// <summary>
        /// Creates a parameterized creator explicitly from an enumerable.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="CreatedType">The type that is created.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <returns>A new parameterized creator.</returns>
        public static IParameterizedCreator<ParameterType, CreatedType> FromEnumerable<ParameterType, CreatedType>(Func<IEnumerable<ParameterType>, IEnumerable<CreatedType>> func)
        {
            return new ParameterizedCreatorFunction<ParameterType, CreatedType>(func);
        }
    }
}

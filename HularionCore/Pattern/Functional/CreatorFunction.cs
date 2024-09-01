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
    /// <typeparam name="CreatedType">The type to create.</typeparam>
    public class CreatorFunction<CreatedType> : ICreator<CreatedType>
    {
        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get { return parameterizedCreator.ProcessPrefixes; } }

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get { return parameterizedCreator.ProcessSuffixes; } }

        /// <summary>
        /// The parameterized crerator has a generalized logic, so we can take advantage of that here.
        /// </summary>
        private ParameterizedCreatorFunction<bool, CreatedType> parameterizedCreator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="creator">The function that will create the instance of CreatedType.</param>
        /// <param name="createInParallel">If true, the objects will be created in parallel.</param>
        public CreatorFunction(Func<CreatedType> creator, bool createInParallel = false)
        {
            var parameterizedCreator = new Func<bool, CreatedType>(x =>
            {
                return creator();
            });

            this.parameterizedCreator = new ParameterizedCreatorFunction<bool, CreatedType>(parameterizedCreator, createInParallel: createInParallel);            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="multipleCreator">The function that will create the instance of T.</param>
        public CreatorFunction(Func<int, IEnumerable<CreatedType>> multipleCreator)
        {
            var parameterizedCreator = new Func<IEnumerable<bool>, IEnumerable<CreatedType>>(x =>
            {
                return multipleCreator(x.Count());
            });
            this.parameterizedCreator = new ParameterizedCreatorFunction<bool, CreatedType>(parameterizedCreator);
        }

        /// <summary>
        /// Creates an instance of T.
        /// </summary>
        /// <returns>The new instance of T.</returns>
        public CreatedType Create()
        {
            return this.parameterizedCreator.Create(true);
        }

        /// <summary>
        /// Creates a number of CreateType objects equal to count.
        /// </summary>
        /// <param name="count">The number of CreateType objects to create.</param>
        /// <returns>The created CreateType objects.</returns>
        public IEnumerable<CreatedType> Create(int count)
        {
            return this.parameterizedCreator.Create(true, count);
        }
    }

    public static class Creator
    {
        /// <summary>
        /// Creates a parameterized creator that explicitly handles a single input at a time.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="CreatedType">The type that is created.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <param name="processesInParallel">If true, the creator is called in parallel when multiple values are created.</param>
        /// <returns>A new parameterized creator.</returns>
        public static ICreator<CreatedType> ForSingle<CreatedType>(Func<CreatedType> func, bool processesInParallel = false)
        {
            return new CreatorFunction<CreatedType>(func, processesInParallel);
        }

        /// <summary>
        /// Creates a parameterized creator explicitly from an enumerable.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="CreatedType">The type that is created.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <returns>A new parameterized creator.</returns>
        public static ICreator< CreatedType> FromEnumerable<ParameterType, CreatedType>(Func<int, IEnumerable<CreatedType>> func)
        {
            return new CreatorFunction<CreatedType>(func);
        }
    }
}


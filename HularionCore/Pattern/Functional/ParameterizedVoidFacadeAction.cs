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
    /// Provides a way to create an IParameterizedFacade from a Func
    /// </summary>
    /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
    public class ParameterizedVoidFacadeAction<ParameterType> : IParameterizedVoidFacade<ParameterType>
    {

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get { return processor.ProcessPrefixes; } }

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get { return processor.ProcessSuffixes; } }


        private ParameterizedFacadeFunction<ParameterType, bool> processor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processor">The function that will execute the facade for each item.</param>
        /// <param name="processesInParallel">If true, collections will be processed in parallel.  Otherwise, they will be processed one member at a time.</param>
        public ParameterizedVoidFacadeAction(Action<ParameterType> processor, bool processesInParallel = false)
        {
            this.processor = new ParameterizedFacadeFunction<ParameterType, bool>(new Func<ParameterType, bool>(parameter =>
            {
                processor(parameter);
                return true;
            }), processesInParallel: processesInParallel);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processor">The function that will execute the facade for a collection of items.</param>
        public ParameterizedVoidFacadeAction(Action<IEnumerable<ParameterType>> processor)
        {
            this.processor = new ParameterizedFacadeFunction<ParameterType, bool>(new Func<IEnumerable<ParameterType>, IEnumerable<bool>>(parameters =>
            {
                processor(parameters);
                return new List<bool>();
            }));
        }

        /// <summary>
        /// Executes the provided facade function.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the facade function.</param>
        public void Process(ParameterType parameter)
        {
            this.processor.Process(parameter);
        }

        /// <summary>
        /// Creates a task that will execute the facade.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the facade.</param>
        /// <returns>A task that will process the parameter.</returns>
        public Task ProcessAsync(ParameterType parameter)
        {
            return new Task(() =>
            {
                this.processor.Process(parameter);
            });
        }

        /// <summary>
        /// Does some processing based on the parameter collection and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        public void Process(IEnumerable<ParameterType> parameters)
        {
            this.processor.Process(parameters);
        }

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>A task that will process the parameters when started.</returns>
        public Task ProcessAsync(IEnumerable<ParameterType> parameters)
        {
            return new Task(() =>
            {
                this.processor.Process(parameters);
            });
        }
    }

    /// <summary>
    /// Creates parameterized void facades using explicit parameters.
    /// </summary>
    public static class ParameterizedVoidFacade
    {
        /// <summary>
        /// Creates a parameterized void facade that explicitly handles a single input at a time.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <param name="processesInParallel">If true, the void facade is called in parallel when multiple values are created.</param>
        /// <returns>A new parameterized void facade.</returns>
        public static IParameterizedVoidFacade<ParameterType> FromSingle<ParameterType>(Action<ParameterType> func, bool processesInParallel = false)
        {
            return new ParameterizedVoidFacadeAction<ParameterType>(func, processesInParallel);
        }

        /// <summary>
        /// Creates a parameterized void facade explicitly from an enumerable.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <returns>A new parameterized void facade.</returns>
        public static IParameterizedVoidFacade<ParameterType> FromEnumerable<ParameterType>(Action<IEnumerable<ParameterType>> func)
        {
            return new ParameterizedVoidFacadeAction<ParameterType>(func);
        }
    }
}

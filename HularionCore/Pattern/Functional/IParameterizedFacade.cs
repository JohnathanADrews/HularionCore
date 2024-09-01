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
    /// A facade that take a parameter, does some processing, and returns a result.
    /// </summary>
    /// <typeparam name="ResultType"></typeparam>
    /// <typeparam name="ParameterType">THe type of the parameter.</typeparam>
    public interface IParameterizedFacade<ParameterType, ResultType> : 
        IProcessorActionAffixer, 
        IProcessCollectionActionAffixer,
        IParameterTransformer<ParameterType>,
        IResultTransformer<ResultType>
    {
        
        /// <summary>
        /// Does some processing based on the parameter and returns the result.
        /// </summary>
        /// <param name="parameter">The parameter to use for processing.</param>
        /// <returns>The result of the processing.</returns>
        ResultType Process(ParameterType parameter);

        /// <summary>
        /// Does some processing asynchronously and returns the result.
        /// </summary>
        /// <param name="parameter">The parameter to use for processing.</param>
        /// <param name="start">Starts the task if and only if true.</param>
        /// <returns>A task that will process the parameter when started.</returns>
        Task<ResultType> ProcessAsync(ParameterType parameter, bool start = false);

        /// <summary>
        /// Does some processing based on the parameter collection and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>The results of the processing.</returns>
        IEnumerable<ResultType> Process(IEnumerable<ParameterType> parameters);

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <param name="start">Starts the task if and only if true.</param>
        /// <returns>A task that will process the parameters when started.</returns>
        Task<IEnumerable<ResultType>> ProcessAsync(IEnumerable<ParameterType> parameters, bool start = false);

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <param name="start">Starts the task if and only if true.</param>
        /// <returns>A task for each parameter that will process the parameters when started.</returns>
        Task<ResultType>[] ProcessEachAsync(IEnumerable<ParameterType> parameters, bool start = false);

        /// <summary>
        /// Does some processing based on the parameter collection and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>The results of the processing.</returns>
        IEnumerable<ResultType> Process(params ParameterType[] parameters);

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="start">Starts the tasks if and only if true.</param>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>A task that will process the parameters when started.</returns>
        Task<IEnumerable<ResultType>> ProcessAsync(bool start = false, params ParameterType[] parameters);

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="start">Starts the tasks if and only if true.</param>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>A task for each parameter that will process the parameters when started.</returns>
        Task<ResultType>[] ProcessEachAsync(bool start = false, params ParameterType[] parameters);

    }
}

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
    /// Converts a function to a facade.
    /// </summary>
    /// <typeparam name="ResultType">The type that is returned by the facade and function.</typeparam>
    public class FacadeFunction<ResultType> : IFacade<ResultType>
    {

        private Func<ResultType> function;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="function">The function that will act as the facade.</param>
        public FacadeFunction(Func<ResultType> function)
        {
            this.function = function;
        }

        /// <summary>
        /// Executes the facade.
        /// </summary>
        /// <returns></returns>
        public ResultType Process()
        {
            return this.function();
        }

        /// <summary>
        /// Returns a task that will execute the facade.
        /// </summary>
        /// <returns>A task that will execute the facade.</returns>
        public Task<ResultType> ProcessAsync()
        {
            return new Task<ResultType>(() =>
            {
                return this.function();
            });
        }
    }
}

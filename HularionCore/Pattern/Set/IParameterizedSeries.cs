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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionCore.Pattern.Set
{
    /// <summary>
    /// A series that has input parameters at each node.
    /// </summary>
    /// <typeparam name="T">The type of value in the series.</typeparam>
    public interface IParameterizedSeries<T> : ISeries<T>
    {

        /// <summary>
        /// The parameters that the values in the series require.
        /// </summary>
        IEnumerable<IParameter> Parameters { get; }

        /// <summary>
        /// Sets the input for the series value.
        /// </summary>
        /// <remarks>
        /// There may be multiple inputs for each value, so an input index is required.
        /// </remarks>
        /// <typeparam name="S">The type of value to set.</typeparam>
        /// <param name="parameter">The parameter information.</param>
        /// <param name="input">The series providing the parameter.</param>
        void SetInput<S>(IParameter parameter, ISeries<S> input);

    }
}

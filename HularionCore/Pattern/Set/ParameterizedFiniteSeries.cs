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
    /// A parameterized finite series using a given series and an Action that sets the parameter.
    /// </summary>
    /// <typeparam name="T">The type of value in the series.</typeparam>
    public class ParameterizedFiniteSeries<T> : IParameterizedFiniteSeries<T>
    {
        /// <summary>
        /// The parameters for each value in the series.
        /// </summary>
        public IEnumerable<IParameter> Parameters { get { return this.parameters; } }

        /// <summary>
        /// The length of the series.
        /// </summary>
        public int Length {  get { return this.series.Length; } }

        /// <summary>
        /// The indices of the series.
        /// </summary>
        public IEnumerable<int> Indices {  get { return this.series.Indices; } }

        /// <summary>
        /// The values of the series.
        /// </summary>
        public IDictionary<int, T> Values { get { return this.series.Values; } }

        /// <summary>
        /// Gets the value of the series at the given index.
        /// </summary>
        /// <param name="index">The index at which to get the value.</param>
        /// <returns>The value of the series at the given index.</returns>
        public T GetValue(int index)
        {
            return this.series.GetValue(index);
        }

        /// <summary>
        /// Sets the input for the series value.
        /// </summary>
        /// <typeparam name="S">The type of value to set.</typeparam>
        /// <param name="parameter">The parameter information.</param>
        /// <param name="input">The value of the input.</param>
        public void SetInput<S>(IParameter parameter, IFiniteSeries<S> input)
        {
            this.parameterSetter(parameter, input);
        }

        /// <summary>
        /// The base series to use.
        /// </summary>
        IFiniteSeries<T> series;

        /// <summary>
        /// The parameters of the values.
        /// </summary>
        IEnumerable<IParameter> parameters;

        /// <summary>
        /// The action that sets the parameters.
        /// </summary>
        Action<IParameter, object> parameterSetter;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="series">The base series.</param>
        /// <param name="parameters">The parameters of the values.</param>
        /// <param name="parameterSetter">The action that sets the parameters.</param>
        public ParameterizedFiniteSeries(IFiniteSeries<T> series, IEnumerable<IParameter> parameters,
            Action<IParameter, object> parameterSetter)
        {
            this.series = series;
            this.parameters = parameters;
            this.parameterSetter = parameterSetter;
        }
    }
}

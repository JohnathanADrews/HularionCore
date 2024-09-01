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

namespace HularionCore.Pattern.Set
{
    /// <summary>
    /// Creates a finite series from a func.
    /// </summary>
    /// <typeparam name="T">The type of value in the series.</typeparam>
    public class FiniteSeriesFunction<T> : IFiniteSeries<T>
    {
        
        /// <summary>
        /// The collection of indices in the finite series.
        /// </summary>
        public IEnumerable<int> Indices { get { return this.finiteSeries.Indices; } }

        /// <summary>
        /// The length of the finite series.
        /// </summary>
        public int Length { get { return this.finiteSeries.Length; } }

        /// <summary>
        /// Get all of the index/value pairs in the finite series.
        /// </summary>
        public IDictionary<int, T> Values { get { return this.finiteSeries.Values; } }

        /// <summary>
        /// Gets the indes of the series provided by the func at the indicated index.
        /// </summary>
        /// <param name="index">The inde at which to get the value.</param>
        /// <returns>The value at the given index.</returns>
        public T GetValue(int index)
        {
            return this.finiteSeries.GetValue(index);
        }
        
        /// <summary>
        /// The finite series that does all of the work.
        /// </summary>
        private IFiniteSeries<T> finiteSeries;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="seriesFunction">The Func that provides the values.</param>
        /// <param name="indices">The collection of indices in the finite series.</param>
        public FiniteSeriesFunction(Func<int, T> seriesFunction, IEnumerable<int> indices)
        {
            var series = new SeriesFunction<T>(seriesFunction);
            this.finiteSeries = new FiniteSeries<T>(series, indices);
        }

        /// <summary>
        /// Constructor using a range of indices.
        /// </summary>
        /// <param name="seriesFunction">The Func that provides the values.</param>
        /// <param name="range">The range of values.</param>
        public FiniteSeriesFunction(Func<int, T> seriesFunction, IInterval<int> range)
        {
            var series = new SeriesFunction<T>(seriesFunction);
            this.finiteSeries = new FiniteSeries<T>(series, range);
        }
    }
}

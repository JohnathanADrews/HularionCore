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
    /// Creates an ISeries using a provided Func
    /// </summary>
    /// <typeparam name="T">The type that the series provides.</typeparam>
    public class SeriesFunction<T> : ISeries<T>
    {
        /// <summary>
        /// The func that provides the series values.
        /// </summary>
        private Func<int, T> seriesProvider = null;

        /// <summary>
        /// Determines whether to cached the series values.
        /// </summary>
        private bool cacheValues;

        /// <summary>
        /// Contains the values to return if they are cached.
        /// </summary>
        private Dictionary<int, T> cache = new Dictionary<int, T>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="seriesProvider">The func that provides the series values.</param>
        /// <param name="cacheValues">Iff true, the series values are generated only once on demand.</param>
        public SeriesFunction(Func<int, T> seriesProvider, bool cacheValues = false)
        {
            this.seriesProvider = seriesProvider;
            this.cacheValues = cacheValues;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="seriesProvider">The provider that provides the series values.</param>
        /// <param name="cacheValues">Iff true, the series values are generated only once on demand.</param>
        public SeriesFunction(IParameterizedProvider<int, T> seriesProvider, bool cacheValues = false)
        {
            this.seriesProvider = new Func<int, T>(x => seriesProvider.Provide(x));
            this.cacheValues = cacheValues;
        }

        /// <summary>
        /// Gets the indes of the series provided by the func at the indicated index.
        /// </summary>
        /// <param name="index">The inde at which to get the value.</param>
        /// <returns>The value at the given index.</returns>
        public T GetValue(int index)
        {
            if(this.cacheValues)
            {
                if(!this.cache.ContainsKey(index))
                {
                    this.cache.Add(index, this.seriesProvider(index));
                }
                return this.cache[index];
            }
            return this.seriesProvider(index);
        }
    }
}

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
    /// A series that can cache and clear values.
    /// </summary>
    /// <typeparam name="T">The type of the series value.</typeparam>
    public class CachedSeries<T> : ISeries<T>
    {
        /// <summary>
        /// Contains all of the cached values.
        /// </summary>
        private Dictionary<int, T> cache = new Dictionary<int, T>();

        /// <summary>
        /// Gets the value at the indicated index.
        /// </summary>
        /// <param name="index">The index at which to get the value.</param>
        /// <returns>The value at the indicated index.</returns>
        public T GetValue(int index)
        {
            if(!this.cache.ContainsKey(index))
            {
                this.cache.Add(index, this.series.GetValue(index));
            }
            return this.cache[index];
        }

        /// <summary>
        /// Clears the series cache.
        /// </summary>
        public void Clear()
        {
            this.cache.Clear();
        }

        /// <summary>
        /// The base series that provides the values.
        /// </summary>
        private ISeries<T> series;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="series">The base series that provides the values.</param>
        public CachedSeries(ISeries<T> series)
        {
            this.series = series;
        }
    }
}

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
    /// A finite set of related values that are indexed.
    /// </summary>
    /// <typeparam name="T">The type of value in the series.</typeparam>
    public interface IFiniteSeries<T>
    {
        /// <summary>
        /// Gets the value of the series at the indicated index.
        /// </summary>
        /// <param name="index">The index of the value to get.</param>
        /// <returns></returns>
        T GetValue(int index);

        /// <summary>
        /// The length of the series.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets all of the indices in the finite series.
        /// </summary>
        IEnumerable<int> Indices { get;  }

        /// <summary>
        /// Get all of the index/value pairs in the finite series.
        /// </summary>
        IDictionary<int, T> Values { get;  }
    }
}

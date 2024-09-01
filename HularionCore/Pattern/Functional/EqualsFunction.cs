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
    /// Creates an instance of IEquals using a provided Func.
    /// </summary>
    /// <typeparam name="ComparedType">The type of objects to compare.</typeparam>
    public class EqualsFunction<ComparedType> : IEquals<ComparedType>
    {

        private Func<ComparedType, ComparedType, bool> equalityDeterminer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="equalityDeterminer">A function that determines whether two values are equal.</param>
        public EqualsFunction(Func<ComparedType, ComparedType, bool> equalityDeterminer)
        {
            this.equalityDeterminer = equalityDeterminer;
        }

        /// <summary>
        /// Determines whether the two values are equivalent.
        /// </summary>
        /// <param name="value1">The first value to compare</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the values are the same.</returns>
        public bool Equals(ComparedType value1, ComparedType value2)
        {
            return this.equalityDeterminer(value1, value2);
        }
    }
}

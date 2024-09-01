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
    /// Specifies a value interval.
    /// </summary>
    /// <typeparam name="BoundaryType">The type of value in the range.</typeparam>
    public class Interval<BoundaryType> : IInterval<BoundaryType>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Interval()
        {
            IncludeLow = true;
            IncludeHigh = true;
        }

        /// <summary>
        /// The low end of the range.
        /// </summary>
        public BoundaryType Low { get; set; }

        /// <summary>
        /// The high end of the range.
        /// </summary>
        public BoundaryType High { get; set; }

        /// <summary>
        /// Iff true, low value is included in the range.
        /// </summary>
        public bool IncludeLow { get; set; }

        /// <summary>
        /// Iff true, high value is included in the range.
        /// </summary>
        public bool IncludeHigh { get; set; }

        private static readonly string leftInclude = "[";
        private static readonly string leftExclude = "(";
        private static readonly string rightInclude = "]";
        private static readonly string rightExclude = ")";
        private static readonly string commaSeparator = ",";

        public static IInterval<BoundaryType> Parse(string text, IParameterizedFacade<string, BoundaryType> valueParser, int? commaSeparatorIndex = null)
        {
            var range = new Interval<BoundaryType>() { IncludeLow = true, IncludeHigh = true };
            var left = text.IndexOf(leftInclude);
            if (left == -1)
            {
                left = text.IndexOf(leftExclude);
                range.IncludeLow = false;
            }
            var right = text.IndexOf(rightInclude);
            if (left == -1)
            {
                right = text.IndexOf(rightExclude);
                range.IncludeHigh = false;
            }
            var commaIndex = text.IndexOf(commaSeparator);
            if(left == -1 || right == -1 || commaIndex == -1)
            {
                throw new ArgumentException(String.Format("The provided interval, \"{0}\" could not be parsed. {1}{2}{3}", text,
                    left == -1 ? "There is no left boundary. " : string.Empty,
                    right == -1 ? "There is no right boundary. " : string.Empty,
                    commaIndex == -1 ? "There is no comma. " : string.Empty));
            }
            if(text.IndexOf(commaSeparator, commaIndex + 1) >= 0)
            {
                if(commaSeparatorIndex == null)
                {
                    throw new ArgumentException("The commaSeparatorIndex must be provided when there are multiple commas in the text parameter.");
                }
                commaIndex = (int)commaSeparatorIndex;
            }
            range.Low = valueParser.Process(text.Substring(left + 1, commaIndex - left - 1));
            range.High = valueParser.Process(text.Substring(commaIndex + 1, right - commaIndex - 1));
            return range;
        }

        public override string ToString()
        {
            return String.Format("{0}{1},{2}{3}", IncludeLow ? leftInclude : leftExclude, Low, High, IncludeHigh ? rightInclude : rightExclude);
        }

        public bool IsCongruent(IInterval<BoundaryType> range)
        {
            return (range.IncludeLow == this.IncludeLow
                && range.IncludeHigh == this.IncludeHigh
                && range.Low.Equals(this.Low)
                && range.High.Equals(this.High));
        }

        public bool IsCongruent(IInterval<BoundaryType> range, IEquals<BoundaryType> equals)
        {
            return (range.IncludeLow == this.IncludeLow
                && range.IncludeHigh == this.IncludeHigh
                && equals.Equals(range.Low, this.Low)
                && equals.Equals(range.High, this.High));
        }

    }
}

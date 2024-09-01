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
    /// A calculator the uses a constructor-injected func to perform the calculation.
    /// </summary>
    /// <typeparam name="ArgumentType">The type of the argument in the calculation.</typeparam>
    /// <typeparam name="CalculatedType">The type of the calculated result.</typeparam>
    public class CalculatorFunction<ArgumentType, CalculatedType> : ICalculator<ArgumentType, CalculatedType>
    {
        /// <summary>
        /// The func that will perform the calculation.
        /// </summary>
        private Func<ArgumentType, CalculatedType> calculator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="calculator">The func that will perform the calculation.</param>
        public CalculatorFunction(Func<ArgumentType, CalculatedType> calculator)
        {
            this.calculator = calculator;
        }

        /// <summary>
        /// Performs the calculation using the defined func.
        /// </summary>
        /// <param name="argument">The calculation's argument.</param>
        /// <returns>The result of the calculation.</returns>
        public CalculatedType Calculate(ArgumentType argument)
        {
            return this.calculator(argument);
        }
    }
}

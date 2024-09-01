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
using System.Text;

namespace HularionCore.Logic
{
    /// <summary>
    /// A binary-valued operator taking two ordered binary values.
    /// </summary>
    public interface IBinaryOperator
    {
        /// <summary>
        /// The name of this operator.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Evaluates the provided parameters against the implementing operator.
        /// </summary>
        /// <param name="p">The first (or left) operand.</param>
        /// <param name="q">The second (or right) operand.</param>
        /// <returns>The result of the evaluation.</returns>
        bool Evaluate(IProvider<bool> p, IProvider<bool> q);

        /// <summary>
        /// Evaluates the parameters against the implementing operator.
        /// </summary>
        /// <param name="p">The first (or left) operand.</param>
        /// <param name="q">The second (or right) operand.</param>
        /// <returns>The result of the evaluation.</returns>
        bool Evaluate(bool a, bool b);

    }
}

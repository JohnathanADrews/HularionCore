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
    /// Implements ISelector with the constructor-injected func.
    /// </summary>
    /// <typeparam name="StateType">The type of the state of being.</typeparam>
    /// <typeparam name="SelectionType">The type of the selection.</typeparam>
    public class SelectorFunction<StateType, SelectionType> : ISelector<StateType, SelectionType>
    {
        /// <summary>
        /// The func that will make the selection.
        /// </summary>
        private Func<StateType, SelectionType> selector;

        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="selector">The func that will make the selection.</param>
        public SelectorFunction(Func<StateType, SelectionType> selector)
        {
            this.selector = selector;
        }

        /// <summary>
        /// Makes the selection given the func set in the constructor.
        /// </summary>
        /// <param name="state">The current state of being.</param>
        /// <returns>The selection.</returns>
        public SelectionType Select(StateType state)
        {
            return this.selector(state);
        }
    }
}

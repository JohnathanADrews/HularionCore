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
    /// A facade that does some processing.
    /// </summary>
    public class VoidFacadeAction : IVoidFacade
    {

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get; private set; } = new ActionCollection();

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get; private set; } = new ActionCollection();

        /// <summary>
        /// The action that will process the facade.
        /// </summary>
        private Action processor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processor">The action that will process the facade.</param>
        public VoidFacadeAction(Action processor)
        {
            this.processor = () =>
            {
                ProcessPrefixes.CallInOrder();
                processor();
                ProcessSuffixes.CallInOrder();
            };
        }

        /// <summary>
        /// Does some processing and returns the result.
        /// </summary>
        public void Process()
        {
            this.processor();
        }

        /// <summary>
        /// Does some processing asynchronously and returns the result.
        /// </summary>
        /// <returns>A task that will run the process.</returns>
        public Task ProcessAsync()
        {
            return new Task(this.processor);
        }
    }
}

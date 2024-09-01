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
    /// An interface for creating objects.
    /// </summary>
    /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
    /// <typeparam name="CreatedType">The type to create.</typeparam>
    public interface IParameterizedCreator<ParameterType, CreatedType> : IProcessorActionAffixer
    {
        /// <summary>
        /// Creates an instance of type CreatedType.
        /// </summary>
        /// <param name="parameter">The parameter used in creation.</param>
        /// <returns>The new instance of type CreatedType.</returns>
        CreatedType Create(ParameterType parameter);

        /// <summary>
        /// Creates instances of type CreatedType.
        /// </summary>
        /// <param name="parameters">The parameters used in creation.</param>
        /// <returns>A new instance of CreatedType for each parameter provided.</returns>
        IEnumerable<CreatedType> Create(IEnumerable<ParameterType> parameters);

        /// <summary>
        /// Creates instances of type CreatedType.
        /// </summary>
        /// <param name="parameter">The parameter used in creation of all created objects.</param>
        /// <param name="count">The number of objects to create.</param>
        /// <returns>The new instances of type CreatedType.</returns>
        IEnumerable<CreatedType> Create(ParameterType parameter, int count);
    }
}

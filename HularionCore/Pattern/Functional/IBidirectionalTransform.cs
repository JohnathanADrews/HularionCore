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
    /// Transforms valus of type S to values of type T, and vice versa.
    /// </summary>
    /// <typeparam name="S">One of the types to transform from or to.</typeparam>
    /// <typeparam name="T">One of the types to transform from or to.</typeparam>
    public interface IBidirectionalTransform<S, T>
    {
        /// <summary>
        /// Transforms a value of type S to a value of type T.
        /// </summary>
        ITransform<S,T> STTransform { get; }

        /// <summary>
        /// Transforms a value of type T to a value of type S.
        /// </summary>
        ITransform<T, S> TSTransform { get; }

    }
}

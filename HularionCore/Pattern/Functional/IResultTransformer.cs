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
using System.Text;

namespace HularionCore.Pattern.Functional
{

    /// <summary>
    /// Enables a processor to transform its parameters prior to processing.
    /// </summary>
    /// <typeparam name="ResultType">The type of the result.</typeparam>
    public interface IResultTransformer<ResultType>
    {
        /// <summary>
        /// Adds a parameter transform to a processor a the beginning of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a result after the process ends.</param>
        void AddResultTransformPrefix(ITransform<ResultType, ResultType> transform);

        /// <summary>
        /// Adds a parameter transform to a processor a the end of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a result after the process ends.</param>
        void AddResultTransformSuffix(ITransform<ResultType, ResultType> transform);

        /// <summary>
        /// Clears all of the parameter transforms.
        /// </summary>
        void ClearResultTransforms();

    }
}

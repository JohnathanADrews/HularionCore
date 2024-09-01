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

namespace HularionCore.Injector
{
    /// <summary>
    /// The modes for injecting values into an injector.
    /// </summary>
    public enum InjectorOverwriteMode
    {
        /// <summary>
        /// Overwrites the current injected value only if the new value is not null and the current value is not null.
        /// </summary>
        ValueOverValue,
        /// <summary>
        /// Overwrites the current value with any new value, including null, but only if the current value is not null.
        /// </summary>
        AnyOverValue,
        /// <summary>
        /// Overwrites the current value only if the new value is null.
        /// </summary>
        NullOverValue,
        /// <summary>
        /// Overwrites the current value only if the current value is null. Does not overwrite non-null values.
        /// </summary>
        ValueOverNull,
        /// <summary>
        /// Overwites any current value with any new value.
        /// </summary>
        AnyOverAny
    }
}

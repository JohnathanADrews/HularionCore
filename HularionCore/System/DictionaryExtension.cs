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

namespace HularionCore.System
{
    /// <summary>
    /// Extensions for dictionaries
    /// </summary>
    public static class DictionaryExtension
    {

        /// <summary>
        /// Gets the value of the dictionary or locks it and adds the provided value if the key does not exist.
        /// </summary>
        /// <typeparam name="KeyType">The type of the key.</typeparam>
        /// <typeparam name="ValueType">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="valueProvider">The value provider in case the key/value pair is not in the dictionary.</param>
        /// <returns></returns>
        public static ValueType LockAddGet<KeyType, ValueType>(this IDictionary<KeyType, ValueType> dictionary, KeyType key, Func<ValueType> valueProvider)
        {
            if (!dictionary.ContainsKey(key))
            {
                lock (dictionary)
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        var value = valueProvider();
                        dictionary.Add(key, value);
                    }
                }
            }
            return dictionary[key];
        }

    }
}

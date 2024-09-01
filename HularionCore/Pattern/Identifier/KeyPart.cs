#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace HularionCore.Pattern.Identifier
{
    /// <summary>
    /// Represents a part of an object key.
    /// </summary>
    public class KeyPart
    {
        /// <summary>
        /// The name of the key part.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of th ekey part.
        /// </summary>
        public object Value { get { if (Key != null) { return Key; } return String; } }

        /// <summary>
        /// The string value of the key part if it is a string and null otherwise.
        /// </summary>
        public string String { get; set; }

        /// <summary>
        /// The ObjectKey value of the key part if it is a ObjectKey and null otherwise.
        /// </summary>
        public ObjectKey Key { get; set; }

        /// <summary>
        /// The type of key part that is the value. 
        /// </summary>
        public KeyPartType KeyPartType { get; set; }

        /// <summary>
        /// true iff the value is not null.
        /// </summary>
        public bool HasPartial { get { return (Value == null); } }

        /// <summary>
        /// The String value if a string or the serialied Key of an ObjectKey.
        /// </summary>
        public string Serialized { get { if (KeyPartType == KeyPartType.String) { return String; } return Key.Serialized; } }

        private static MemberMapper mapper = new MemberMapper();

        static KeyPart()
        {
            mapper.CreateMap<KeyPart, KeyPart>();
        }

        /// <summary>
        /// Clones this Keypart.
        /// </summary>
        /// <returns>A copy of this.</returns>
        public KeyPart Clone()
        {
            var clone = new KeyPart();
            mapper.Map(this, clone);
            return clone;
        }

        /// <summary>
        /// Creates a new key from this KeyPart.
        /// </summary>
        /// <returns>An ObjectKey with the key information.</returns>
        public ObjectKey ToKey()
        {
            var key = new ObjectKey();
            if(KeyPartType == KeyPartType.String)
            {
                key.SetPart(Name, String);
            }
            if(KeyPartType == KeyPartType.Object)
            {
                key.SetPart(Name, Key);
            }
            return key;
        }
    }
}

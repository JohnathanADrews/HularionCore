#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.Pattern.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HularionCore.Pattern.Identifier
{
    /// <summary>
    /// A multi-part key.
    /// </summary>
    public class ObjectKey
    {
        public string KeyDelimiter { get; set; } = "+";
        public string PartialSeparator { get; set; } = ":";
        public string BeginNestedKey { get; set; } = "[";
        public string EndNestedKey { get; set; } = "]";


        /// <summary>
        /// The serialized version of the key.
        /// </summary>
        public string Serialized
        {
            get 
            {
                Serialize();
                return serialized; 
            }
            set
            { 
                this.serialized = value == null ? NullKey.serialized : value;  
                this.isSerialized = true;
                this.isParsed = false;
            }
        }

        private bool isParsed = false;
        private bool isSerialized = false;

        /// <summary>
        /// A null-value key
        /// </summary>
        public static ObjectKey NullKey { get; set; } = ObjectKey.Parse("NullKey");

        private static Type keyType = typeof(ObjectKey);
        private static Type stringType = typeof(string);

        private string serialized = string.Empty;

        private Dictionary<string, KeyPart> parts = new Dictionary<string, KeyPart>();


        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectKey()
        {
        }


        /// <summary>
        /// Clones this key.
        /// </summary>
        /// <returns>A copy of this key.</returns>
        public ObjectKey Clone()
        {
            return new ObjectKey() { serialized = String.Format("{0}", serialized) };
        }

        /// <summary>
        /// Adds the specified key part and value.
        /// </summary>
        /// <param name="partKey">The key of the part.</param>
        /// <param name="partValue">The value of the part.</param>
        /// <returns>The key part that was set.</returns>
        public KeyPart SetPart(string partKey, string partValue)
        {
            UpdateParts();
            var part = new KeyPart() { Name = partKey, String = partValue, KeyPartType = KeyPartType.String };
            parts[partKey] = part;
            return part;
        }

        /// <summary>
        /// Adds the specified key part and value.
        /// </summary>
        /// <param name="partKey">The key of the part.</param>
        /// <param name="partValue">The sub-key of the part.</param>
        /// <returns>The key part that was set.</returns>
        public KeyPart SetPart(string partKey, ObjectKey partValue)
        {
            UpdateParts();
            var part = SetPartPrivate(partKey, partValue);
            return part;
        }

        private KeyPart SetPartPrivate(string partKey, ObjectKey partValue)
        {
            isSerialized = false;
            var part = new KeyPart() { Name = partKey, Key = partValue, KeyPartType = KeyPartType.Object };
            parts[partKey] = part;
            return part;
        }

        private KeyPart SetPartPrivate(string partKey, string partValue)
        {
            isSerialized = false;
            var part = new KeyPart() { Name = partKey, String = partValue, KeyPartType = KeyPartType.String };
            parts[partKey] = part;
            return part;
        }



        /// <summary>
        /// Gets the key part indicated by the given name or null if the part was not found.
        /// </summary>
        /// <returns>The specified part or null if the part was not found.</returns>
        public KeyPart GetKeyPart(string partName)
        {
            UpdateParts();
            if (!parts.ContainsKey(partName)) { return null; }
            var part = parts[partName];
            return part.Clone();
        }

        /// <summary>
        /// Updates the parts dictionary if the serialized version has been set and no update has been performed.
        /// </summary>
        public void UpdateParts()
        {
            if (isParsed) { return; }
            if (String.IsNullOrWhiteSpace(serialized)) { return; }

            parts.Clear();

            var stack = new Stack<ObjectKey>();
            stack.Push(this);

            int index = 0;
            int startNestIndex;
            int endNestIndex;

            Func<ObjectKey, int, int, bool, ObjectKey> parser = (key, startIndex, stopIndex, isStart) =>
            {
                var section = serialized.Substring(startIndex, stopIndex - startIndex);
                var keys = section.Split(new string[] { KeyDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                for(var i = 0; i < keys.Length + (isStart ? -1 : 0); i++)
                {
                    var split = keys[i].Split(new string[] { PartialSeparator }, StringSplitOptions.None);
                    key.SetPartPrivate(split[0], split.Length > 1 ? split[1] : null);
                }
                ObjectKey keyResult = null;
                if (isStart)
                {
                    keyResult = key.MakeKey();
                    var name = keys.Last().Split(new string[] { PartialSeparator }, StringSplitOptions.None).First();
                    key.SetPartPrivate(name, keyResult);
                }
                return keyResult;
            };

            while(index >= 0)
            {
                endNestIndex = serialized.IndexOf(EndNestedKey, index);
                startNestIndex = serialized.IndexOf(BeginNestedKey, index);

                if(startNestIndex < 0 && endNestIndex < 0) { break; }

                if(startNestIndex < 0 && endNestIndex >= 0)
                {
                    var key = stack.Pop();
                    parser(key, index, endNestIndex, false);
                    index = endNestIndex + EndNestedKey.Length;
                    continue;
                }
                if(endNestIndex < startNestIndex)
                {
                    var key = stack.Pop();
                    parser(key, index, endNestIndex, false);
                    index = endNestIndex + EndNestedKey.Length;
                }
                else
                {
                    var key = parser(stack.Peek(), index, startNestIndex, true);
                    stack.Push(key);
                    index = startNestIndex + BeginNestedKey.Length;
                }
            }

            if(index < serialized.Length)
            {
                parser(this, index, serialized.Length, false);
            }
            isParsed = true;
        }

        /// <summary>
        /// Creates a new key.
        /// </summary>
        /// <returns>A new key.</returns>
        public ObjectKey MakeKey()
        {
            var key = new ObjectKey() { BeginNestedKey = BeginNestedKey, EndNestedKey = EndNestedKey, KeyDelimiter = KeyDelimiter, PartialSeparator = PartialSeparator };
            return key;
        }

        /// <summary>
        /// Serializes the key if it has not been serializ2ed since the last changes.
        /// </summary>
        private void Serialize()
        {
            if (isSerialized) { return; }
            var root = new KeyPart() { Key = this, KeyPartType = KeyPartType.Object };
            var traverser = new TreeTraverser<KeyPart>();
            var keyTraverser = new TreeTraverser<ObjectKey>();

            var result = new StringBuilder();
            traverser.WeaveExecute(TreeWeaveOrder.FromLeft, root, node =>
                {
                    if(node.KeyPartType == KeyPartType.String) { return new KeyPart[] { }; }
                    var next = ((ObjectKey)node.Value).parts.OrderBy(x => x.Key).Select(x => x.Value.Clone()).ToArray();
                    return next;
                },
                entryAction: state=>
                {
                    if(state.Subject == root) { return; }
                    result.Append(state.Subject.Name);
                    result.Append(PartialSeparator);
                    if (state.Subject.KeyPartType == KeyPartType.String) { result.Append(state.Subject.Value); }
                    if(state.Subject.KeyPartType == KeyPartType.Object) { result.Append(BeginNestedKey); }                    
                },
                upAction: state =>
                {
                },
                exitAction: state =>
                {
                    if (state.IsRowLast) { return; }
                    result.Append(KeyDelimiter);
                },
                lastAction: state=>
                {
                    //if (state.Subject == root || state.Subject.KeyPartType == KeyPartType.String) { return; }
                    if (state.Subject == root || state.Parent == root) { return; }
                    result.Append(EndNestedKey);
                });

            serialized = result.ToString();
            isSerialized = true;
        }

        /// <summary>
        /// Determines whether the provided key is equivalent to this key.
        /// </summary>
        /// <param name="key">The key to compare.</param>
        /// <returns>true iff the given key is equivalent to this key.</returns>
        public bool EqualsKey(ObjectKey key)
        {
            if(key == null) { return false; }
            return CheckEquals(key.serialized);
        }

        /// <summary>
        /// Determines whether the provided object is equivalent to this key.
        /// </summary>
        /// <param name="key">The object to compare.</param>
        /// <returns>true iff the given object is equivalent to this key.</returns>
        public bool EqualsKey(object key)
        {
            if(key == null) { return false; }
            var type = key.GetType();
            if (type == stringType) { return CheckEquals((string)key); }
            if (keyType.IsAssignableFrom(type)) { return CheckEquals(((ObjectKey)key).serialized); }
            return false;
        }

        /// <summary>
        /// Determines whether the provided string is equivalent to this key.
        /// </summary>
        /// <param name="key">The string to compare.</param>
        /// <returns>true iff the given string is equivalent to this key.</returns>
        public bool EqualsKey(string key)
        {
            return CheckEquals(key);
        }

        private bool CheckEquals(string serializedKey)
        {
            if (!isSerialized) { Serialize(); }
            return (serialized == serializedKey);
        }

        static byte keyCharReplacer = 0;
        private static char GetCharReplacer()
        {
            return (char)((keyCharReplacer++ & 25) + 97);
        }

        private static string TagFromGuid(Guid guid)
        {
            return Convert.ToBase64String(guid.ToByteArray()).Substring(0, 22).Replace('+', GetCharReplacer()).Replace('/', GetCharReplacer());
        }

        /// <summary>
        /// Creates a unique character string from a GUID.
        /// </summary>
        /// <returns>A unique character string from a GUID.</returns>
        public static string CreateUniqueTag()
        {
            return TagFromGuid(Guid.NewGuid());
        }

        /// <summary>
        /// Creates a unique key from a unique tag.
        /// </summary>
        /// <returns>A unique key from a unique tag.</returns>
        public static ObjectKey CreateUniqueTagKey()
        {
            return ObjectKey.Parse(CreateUniqueTag());
        }

        /// <summary>
        /// Creates a unique key from a unique tag.
        /// </summary>
        /// <param name="keyPart">The part of the key in which to place the tag.</param>
        /// <returns>A unique key from a unique tag.</returns>
        public static ObjectKey CreateUniqueTagKey(string keyPart)
        {
            var key = new ObjectKey();
            key.SetPart(keyPart, CreateUniqueTag());
            return key;
        }

        /// <summary>
        /// Parses the given string to create a key.
        /// </summary>
        /// <param name="serialized">The serialized key.</param>
        /// <returns>A key based formed from the given string.</returns>
        public static ObjectKey Parse(string serialized)
        {
            return new ObjectKey() { Serialized = serialized };
        }

        /// <summary>
        /// Parses the given object to create a key.
        /// </summary>
        /// <param name="value">The value containing the key information.</param>
        /// <returns>A key based formed from the given object.</returns>
        public static ObjectKey Parse(object value)
        {
            if (value == null) { return null; }
            var type = value.GetType();
            if (keyType.IsAssignableFrom(type)) { return (ObjectKey)value; }
            if (type == stringType) { return new ObjectKey() { Serialized = String.Format("{0}", value) }; }
            return new ObjectKey() { Serialized = String.Format("0", value.ToString()) };
        }


        public override string ToString()
        {
            return Serialized;
        }

        public override int GetHashCode()
        {
            return Serialized.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return EqualsKey(obj);
        }
    }
}

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

namespace HularionCore.TypeGraph
{
    /// <summary>
    /// Manages and provides TypeNode object.
    /// </summary>
    public class TypeNodeManager
    {
        /// <summary>
        /// Provides a TypeNode given the provided type.
        /// </summary>
        public IParameterizedProvider<Type, TypeNode> TypeNodeProvider { get; private set; }

        private Dictionary<Type, TypeNode> typeNodes = new Dictionary<Type, TypeNode>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public TypeNodeManager(ITransform<string, string> memberNameTransform)
        {
            TypeNodeProvider = ParameterizedProvider.FromSingle<Type, TypeNode>(type => 
            {
                if (typeNodes.ContainsKey(type)) { return typeNodes[type]; }
                lock (typeNodes)
                {
                    if (typeNodes.ContainsKey(type)) { return typeNodes[type]; }
                    var plan = TypeNode.CreateTypeTreePlan(type, memberNameTransform);
                    foreach(var node in plan)
                    {
                        if (!typeNodes.ContainsKey(node.Type)) { typeNodes.Add(node.Type, node); }
                    }
                }
                return typeNodes[type];
            });
        }


        public void SetTypeNode<T>(TypeNode typeNode)
        {
            SetTypeNode(typeof(T), typeNode);
        }

        public void SetTypeNode(Type type, TypeNode typeNode)
        {
            lock (typeNodes)
            {
                typeNodes[type] = typeNode;
            }
        }

        public void ClearMembers<T>()
        {
            TypeNodeProvider.Provide(typeof(T)).Members.Clear();
        }

        public void ClearMembers(params Type[] types)
        {
            foreach(var type in types)
            {
                TypeNodeProvider.Provide(type).Members.Clear();
            }
        }

    }

    
}

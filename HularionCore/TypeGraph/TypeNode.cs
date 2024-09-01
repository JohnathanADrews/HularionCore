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
using HularionCore.Pattern.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HularionCore.TypeGraph
{
    /// <summary>
    /// Describes a type and links it to its members.
    /// </summary>
    public class TypeNode
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// If Type is generic, this is the generic definition.
        /// </summary>
        public Type GenericDefinition { get; set; }

        /// <summary>
        /// The members of this node.
        /// </summary>
        public IDictionary<string, TypeNodeMember> Members { get; set; } = new Dictionary<string, TypeNodeMember>();

        /// <summary>
        /// The members of this node.
        /// </summary>
        public List<TypeNode> Generics { get; set; } = new List<TypeNode>();

        /// <summary>
        /// Creates a tree of TypeNode based on the provided type.
        /// </summary>
        /// <typeparam name="T">The type from which to create the tree.</typeparam>
        /// <returns>A tree of TypeNode based on the provided type.</returns>
        public static TypeNode CreateTypeTree<T>(ITransform<string, string> memberNameTransform)
        {
            return CreateTypeTree(typeof(T), memberNameTransform: memberNameTransform);
        }

        /// <summary>
        /// Creates a tree of TypeNode based on the provided type.
        /// </summary>
        /// <param name="type">The type from which to create the tree.</param>
        /// <returns>A tree of TypeNode based on the provided type.</returns>
        public static TypeNode CreateTypeTree(Type type, ITransform<string, string> memberNameTransform)
        {
            return CreateTypeTreePlan(type, memberNameTransform: memberNameTransform)[0];
        }


        /// <summary>
        /// Creates a PLR plan of TypeNodes based on the provided type.
        /// </summary>
        /// <typeparam name="T">The type from which to create the tree.</typeparam>
        /// <returns>A tree of TypeNode based on the provided type.</returns>
        public static TypeNode[] CreateTypeTreePlan<T>(ITransform<string, string> memberNameTransform)
        {
            return CreateTypeTreePlan(typeof(T), memberNameTransform: memberNameTransform);
        }

        /// <summary>
        /// Creates a PLR evaluation plan of TypeNodes based on the provided type.
        /// </summary>
        /// <param name="type">The type from which to create the tree.</param>
        /// <returns>A tree of TypeNode based on the provided type.</returns>
        public static TypeNode[] CreateTypeTreePlan(Type type, ITransform<string, string> memberNameTransform)
        {
            var traverser = new TreeTraverser<TypeNode>();
            var typeGenericMap = new Dictionary<Type, Type>();

            //var caseModifier = new StringCaseModifier(memberCase);

            var map = new Dictionary<Type, TypeNode>();
            var createNode = new Func<Type, TypeNode>(createType =>
            {
                var result = new TypeNode() { Type = createType, GenericDefinition = createType.IsGenericType ? createType.GetGenericTypeDefinition() : null };
                if (createType.IsGenericType) { typeGenericMap[createType] = createType.GetGenericTypeDefinition(); }
                else { typeGenericMap[createType] = null; }
                map.Add(createType, result);
                return result;
            });
            var root = createNode(type);

            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, root, node =>
            {
                var next = new List<TypeNode>();
                foreach (var property in node.Type.GetNonStaticGetSetProperties())
                {
                    var propertyType = property.PropertyType;
                    var caseName = memberNameTransform.Transform(property.Name);
                    var member = new TypeNodeMember() { Parent = node, Name = caseName, Type = propertyType, Property = property };
                    node.Members[caseName] = member;
                    if (propertyType.IsGenericParameter)
                    {
                        member.IsGenericParameter = true;
                        propertyType = node.Type.GenericTypeArguments.Where(x => x.Name == propertyType.Name).First();
                    }
                    if (map.ContainsKey(propertyType))
                    {
                        member.Member = map[propertyType];
                        node.Members[caseName] = member;
                    }
                    else
                    {
                        var nextNode = createNode(propertyType);
                        member.Member = nextNode;
                        node.Members[caseName] = member;
                        next.Add(nextNode);
                    }
                }
                foreach (var field in node.Type.GetNonStaticFields())
                {
                    var fieldType = field.FieldType;
                    var caseName = memberNameTransform.Transform(field.Name);
                    var member = new TypeNodeMember() { Parent = node, Name = caseName, Type = fieldType, Field = field };
                    node.Members[caseName] = member;
                    if (fieldType.IsGenericParameter)
                    {
                        member.IsGenericParameter = true;
                        fieldType = node.Type.GenericTypeArguments.Where(x => x.Name == fieldType.Name).First();
                    }
                    if (map.ContainsKey(fieldType))
                    {
                        node.Members[caseName] = member;
                    }
                    else
                    {
                        var nextNode = createNode(fieldType);
                        member.Member = nextNode;
                        node.Members[caseName] = member;
                        next.Add(nextNode);
                    }
                }
                foreach(var genericType in node.Type.GetGenericArguments())
                {
                    if (map.ContainsKey(genericType))
                    {
                        node.Generics.Add(map[genericType]);
                    }
                    else
                    {
                        var nextNode = createNode(genericType);
                        node.Generics.Add(nextNode);
                        next.Add(nextNode);
                    }
                }
                if (node.Type.IsArray)
                {
                    var elementType = node.Type.GetElementType();
                    if (map.ContainsKey(elementType))
                    {
                        node.Generics.Add(map[elementType]);
                    }
                    else
                    {
                        var nextNode = createNode(elementType);
                        node.Generics.Add(nextNode);
                        next.Add(nextNode);
                    }
                }
                return next.ToArray();
            }, true);

            return plan;
        }


        public override string ToString()
        {
            return string.Format("TypeNode: {0}", Type.Name);
        }

    }


}

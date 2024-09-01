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
    /// Implements ITransform by calling the Func provided in the constructor.
    /// </summary>
    /// <typeparam name="InputType">The type of the input.</typeparam>
    /// <typeparam name="OutputType">The type of the output.</typeparam>
    public class TransformFunction<InputType, OutputType> : ITransform<InputType, OutputType>
    {

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get { return processor.ProcessPrefixes; } } 

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get { return processor.ProcessSuffixes; } }

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time a collection of items is processed, before the items are processed.
        /// </summary>
        public ActionCollection ProcessCollectionPrefixes { get { return processor.ProcessCollectionPrefixes; } }

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time a collection of items is processed, after the items are processed.
        /// </summary>
        public ActionCollection ProcessCollectionSuffixes { get { return processor.ProcessCollectionSuffixes; } }


        private ParameterizedFacadeFunction<InputType, OutputType> processor;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="transformFunction">The function that will provide the transform.</param>
        /// <param name="transformsInParallel">If true, multiple transformations will be processed in parallel.</param>
        public TransformFunction(Func<InputType, OutputType> transformFunction, bool transformsInParallel = false)
        {
            this.processor = new ParameterizedFacadeFunction<InputType, OutputType>(new Func<InputType, OutputType>(input =>
            {
                return transformFunction(input);
            }), processesInParallel: transformsInParallel);
        }

        /// <summary>
        /// Transforms the input into the an output of OutputType.
        /// </summary>
        /// <param name="input">The input to transform.</param>
        /// <returns>The transformed object.</returns>
        public OutputType Transform(InputType input)
        {
            return this.processor.Process(input);
        }
        
        /// <summary>
        /// Transofrms a collection of input type to output type and returns it..
        /// </summary>
        /// <param name="inputs">The objects to transform to the output type.</param>
        /// <returns>The transformed objects.</returns>
        public IEnumerable<OutputType> Transform(IEnumerable<InputType> inputs)
        {
            return this.processor.Process(inputs);
        }
    }

    /// <summary>
    /// Creates transforms using explicit parameters.
    /// </summary>
    public static class Transform
    {
        /// <summary>
        /// Creates a new transform function from the provided func.
        /// </summary>
        /// <typeparam name="InputType">The type of the input.</typeparam>
        /// <typeparam name="OutputType">The type of the output.</typeparam>
        /// <param name="func">The function that will provide the transform.</param>
        /// <returns>A new transform.</returns>
        public static ITransform<InputType, OutputType> Create<InputType, OutputType>(Func<InputType, OutputType> func)
        {
            return new TransformFunction<InputType, OutputType>(func);
        }

        /// <summary>
        /// Transforms each of the values using the same type to type transfomers
        /// </summary>
        /// <typeparam name="T">The type that is both the input and output type of the transforms.</typeparam>
        /// <param name="transforms">The transforms to apply.</param>
        /// <param name="values">The values to transform.</param>
        /// <returns>The transformed values.</returns>
        public static T[] SameTypeTransformAll<T>(ITransform<T, T>[] transforms, params T[] values)
        {
            if (values == null || values.Length == 0) { return values; }
            var result = new List<T>();
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                for (var j = 0; j < transforms.Length; j++)
                {
                    value = transforms[j].Transform(value);
                }
                result.Add(value);
            }
            return result.ToArray();
        }

    }

}

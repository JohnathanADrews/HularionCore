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
    /// Provides a way to create an IParameterizedFacade from a Func
    /// </summary>
    /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
    /// <typeparam name="ResultType">The type of the result.</typeparam>
    public class ParameterizedFacadeFunction<ParameterType, ResultType> : IParameterizedFacade<ParameterType, ResultType>
    {

        /// <summary>
        /// The provided processing function.
        /// </summary>
        private Func<ParameterType, ResultType> processor;

        /// <summary>
        /// The processing function.
        /// </summary>
        private Func<IEnumerable<ParameterType>, IEnumerable<ResultType>> collectionProcessor;

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, before the items are processed.
        /// </summary>
        public ActionCollection ProcessPrefixes { get; private set; } = new ActionCollection();

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time Process is called, after the items are processed.
        /// </summary>
        public ActionCollection ProcessSuffixes { get; private set; } = new ActionCollection();

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time a collection of items is processed, before the items are processed.
        /// </summary>
        public ActionCollection ProcessCollectionPrefixes { get; private set; } = new ActionCollection();

        /// <summary>
        /// Calls ActionCollection.CallInOrder each time a collection of items is processed, after the items are processed.
        /// </summary>
        public ActionCollection ProcessCollectionSuffixes { get; private set; } = new ActionCollection();


        private List<ITransform<ParameterType, ParameterType>> parameterTransforms { get; set; } = new List<ITransform<ParameterType, ParameterType>>();
        private List<ITransform<ResultType, ResultType>> resultTransforms { get; set; } = new List<ITransform<ResultType, ResultType>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processor">The function that will execute the facade for each item.</param>
        /// <param name="processesInParallel">If true, collections will be processed in parallel.  Otherwise, they will be processed one member at a time.</param>
        public ParameterizedFacadeFunction(Func<ParameterType, ResultType> processor, bool processesInParallel = false)
        {
            this.processor = new Func<ParameterType, ResultType>((parameter) =>
            {
                ProcessPrefixes.CallInOrder();
                var result = processor(parameter);
                ProcessSuffixes.CallInOrder();
                return result;
            });
            if (processesInParallel)
            {
                this.collectionProcessor = new Func<IEnumerable<ParameterType>, IEnumerable<ResultType>>(parameters =>
                {
                    var result = new List<ResultType>();
                    ProcessPrefixes.CallInOrder();
                    Parallel.ForEach(parameters, x =>
                    {
                        var value = processor(x);
                        lock (result)
                        {
                            result.Add(value);
                        }
                    });
                    ProcessSuffixes.CallInOrder();
                    return result;
                });
            }
            else
            {
                this.collectionProcessor = new Func<IEnumerable<ParameterType>, IEnumerable<ResultType>>(x =>
                {
                    ProcessPrefixes.CallInOrder();
                    var result = x.Select(y => processor(y)).ToList();
                    ProcessSuffixes.CallInOrder();
                    return result;
                });
            }

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processor">The function that will execute the facade for a collection of items.</param>
        public ParameterizedFacadeFunction(Func<IEnumerable<ParameterType>, IEnumerable<ResultType>> processor)
        {
            this.collectionProcessor = new Func<IEnumerable<ParameterType>, IEnumerable<ResultType>>(parameter =>
            {
                ProcessPrefixes.CallInOrder();
                var result = processor(parameter);
                ProcessSuffixes.CallInOrder();
                return result;
            });
            //define a single processor that processes a list of one.
            this.processor = new Func<ParameterType, ResultType>(x =>
            {
                ProcessPrefixes.CallInOrder();
                var result = processor(new List<ParameterType>() { x }).FirstOrDefault();
                ProcessSuffixes.CallInOrder();
                return result;
            });


        }

        private ParameterizedFacadeFunction()
        {

        }

        /// <summary>
        /// Creates a copy of this with empty affixes.
        /// </summary>
        /// <returns>A copy of this with empty affixes.</returns>
        public ParameterizedFacadeFunction<ParameterType, ResultType> CopyWithEmptyAffixes()
        {
            var result = new ParameterizedFacadeFunction<ParameterType, ResultType>();
            result.processor = this.processor;
            result.collectionProcessor = this.collectionProcessor;
            return result;
        }

        /// <summary>
        /// Executes the provided facade function.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the facade function.</param>
        /// <returns>The result of the facade.</returns>
        public ResultType Process(ParameterType parameter)
        {
            parameter = TransformParameters(parameter).FirstOrDefault();
            var result = processor(parameter);
            result = TransformResults(result).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Creates a task that will execute the facade.
        /// </summary>
        /// <param name="parameter">The parameter to pass to the facade.</param>
        /// <param name="start">Starts the task if and only if true.</param>
        /// <returns>A task that will execute the facade.</returns>
        public Task<ResultType> ProcessAsync(ParameterType parameter, bool start = false)
        {
            parameter = TransformParameters(parameter).FirstOrDefault();
            var task = new Task<ResultType>(() =>
            {
                var result = this.processor(parameter);
                result = TransformResults(result).FirstOrDefault();
                return result;
            });
            if (start) { task.Start(); }
            return task;
        }

        /// <summary>
        /// Does some processing based on the parameter collection and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>The results of the processing.</returns>
        public IEnumerable<ResultType> Process(IEnumerable<ParameterType> parameters)
        {
            parameters = TransformParameters(parameters);
            ProcessCollectionPrefixes.CallInOrder();
            var result = this.collectionProcessor(parameters);
            result = TransformResults(result);
            ProcessCollectionSuffixes.CallInOrder();
            return result;
        }

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <param name="start">Starts the task if and only if true.</param>
        /// <returns>A task that will process the parameters when started.</returns>
        public Task<IEnumerable<ResultType>> ProcessAsync(IEnumerable<ParameterType> parameters, bool start = false)
        {
            parameters = TransformParameters(parameters);
            var task = new Task<IEnumerable<ResultType>>(() =>
            {
                ProcessCollectionPrefixes.CallInOrder();
                var result = this.collectionProcessor(parameters);
                result = TransformResults(result);
                ProcessCollectionSuffixes.CallInOrder();
                return result;
            });
            if (start) { task.Start(); }
            return task;
        }

        /// <summary>
        /// Does some processing based on the parameter collection and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>The results of the processing.</returns>
        public IEnumerable<ResultType> Process(params ParameterType[] parameters)
        {
            parameters = TransformParameters(parameters);
            var result = this.collectionProcessor(parameters);
            result = TransformResults(result);
            return result;
        }

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>A task that will process the parameters when started.</returns>
        public Task<IEnumerable<ResultType>> ProcessAsync(bool start = false, params ParameterType[] parameters)
        {
            parameters = TransformParameters(parameters);
            var task = new Task<IEnumerable<ResultType>>(() =>
            {
                ProcessCollectionPrefixes.CallInOrder();
                var result = this.collectionProcessor(parameters);
                result = TransformResults(result);
                ProcessCollectionSuffixes.CallInOrder();
                return result;
            });
            if (start) { task.Start(); }
            return task;
        }

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <param name="start">Starts the tasks if and only if true.</param>
        /// <returns>A task for each parameter that will process the parameters when started.</returns>
        public Task<ResultType>[] ProcessEachAsync(IEnumerable<ParameterType> parameters, bool start = false)
        {
            parameters = TransformParameters(parameters);
            if (parameters == null) { return new Task<ResultType>[] { }; }
            var tasks = parameters.Select(parameter => new Task<ResultType>(() => { return TransformResults(this.processor(parameter)).FirstOrDefault(); })).ToArray();
            ProcessCollectionPrefixes.CallInOrder();
            if (start)
            {
                foreach (var task in tasks) { task.Start(); }
            }
            Task.WhenAll(tasks.ToArray()).ContinueWith(new Action<Task>((x) => { ProcessCollectionSuffixes.CallInOrder(); }));
            return tasks;
        }

        /// <summary>
        /// Does some processing asynchronously and returns the results.
        /// </summary>
        /// <param name="start">Starts the tasksif and only if true.</param>
        /// <param name="parameters">The parameters to use for processing.</param>
        /// <returns>A task for each parameter that will process the parameters when started.</returns>
        public Task<ResultType>[] ProcessEachAsync(bool start = false, params ParameterType[] parameters)
        {
            parameters = TransformParameters(parameters);
            if (parameters == null) { return new Task<ResultType>[] { }; }
            var tasks = parameters.Select(parameter => new Task<ResultType>(() => { return TransformResults(this.processor(parameter)).FirstOrDefault(); })).ToArray();
            ProcessCollectionPrefixes.CallInOrder();
            if (start)
            {
                foreach (var task in tasks) { task.Start(); }
            }
            Task.WhenAll(tasks.ToArray()).ContinueWith(new Action<Task>((x) => { ProcessCollectionSuffixes.CallInOrder(); }));
            return tasks;
        }

        /// <summary>
        /// Adds a parameter transform to a processor at the beginning of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a parameter before the process begins.</param>
        public void AddParameterTransformPrefix(ITransform<ParameterType, ParameterType> transform)
        {
            parameterTransforms.Insert(0, transform);
        }

        /// <summary>
        /// Adds a parameter transform to a processor at the end of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a parameter before the process begins.</param>
        public void AddParameterTransformSuffix(ITransform<ParameterType, ParameterType> transform)
        {
            parameterTransforms.Add(transform);
        }

        /// <summary>
        /// Clears all of the parameter transforms.
        /// </summary>
        public void ClearParameterTransforms()
        {
            parameterTransforms.Clear();
        }

        /// <summary>
        /// Adds a parameter transform to a processor a the beginning of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a result after the process ends.</param>
        public void AddResultTransformPrefix(ITransform<ResultType, ResultType> transform)
        {
            resultTransforms.Insert(0, transform);
        }

        /// <summary>
        /// Adds a parameter transform to a processor a the end of the transform sequence.
        /// </summary>
        /// <param name="transform">The transform to execute on a result after the process ends.</param>
        public void AddResultTransformSuffix(ITransform<ResultType, ResultType> transform)
        {
            resultTransforms.Add(transform);
        }

        /// <summary>
        /// Clears all of the parameter transforms.
        /// </summary>
        public void ClearResultTransforms()
        {
            resultTransforms.Clear();
        }

        //public ParameterizedFacadeFunction<PredecessorType, ResultType> ApplyPredecessor<PredecessorType>(IParameterizedFacade<PredecessorType, ParameterType> predecessor)
        //{
        //    return ParameterizedFacade.FromEnumerable<PredecessorType, ResultType>(parameters =>
        //    {
        //        var predecessorResult = predecessor.Process(parameters);
        //        var thisResult = this.Process(predecessorResult);
        //        return thisResult;
        //    });
        //}

        //public ParameterizedFacadeFunction<ParameterType, SuccessorResultType> ApplySuccessor<SuccessorResultType>(IParameterizedFacade<ResultType, SuccessorResultType> successor)
        //{
        //    return ParameterizedFacade.FromEnumerable<ParameterType, SuccessorResultType>(parameters =>
        //    {
        //        var thisResult = this.Process(parameters);
        //        var successorResult = successor.Process(thisResult);
        //        return successorResult;
        //    });
        //}

        private ParameterType[] TransformParameters(params ParameterType[] values)
        {
            if (values == null) { return new ParameterType[] { }; }
            return Transform.SameTypeTransformAll<ParameterType>(parameterTransforms.ToArray(), values);
        }
        private ParameterType[] TransformParameters(IEnumerable<ParameterType> values)
        {
            if(values == null) { return new ParameterType[] { }; }
            return Transform.SameTypeTransformAll<ParameterType>(parameterTransforms.ToArray(), values.ToArray());
        }
        private ResultType[] TransformResults(params ResultType[] values)
        {
            if (values == null) { return new ResultType[] { }; }
            return Transform.SameTypeTransformAll<ResultType>(resultTransforms.ToArray(), values);
        }
        private ResultType[] TransformResults(IEnumerable<ResultType> values)
        {
            if (values == null) { return new ResultType[] { }; }
            return Transform.SameTypeTransformAll<ResultType>(resultTransforms.ToArray(), values.ToArray());
        }

    }

    /// <summary>
    /// Creates parameterized facades using explicit parameters.
    /// </summary>
    public static class ParameterizedFacade
    {
        /// <summary>
        /// Creates a parameterized facade that explicitly handles a single input at a time.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="ResultType">The type of the result.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <param name="processesInParallel">If true, the facade is called in parallel when multiple values are created.</param>
        /// <returns>A new parameterized facade.</returns>
        public static ParameterizedFacadeFunction<ParameterType, ResultType> FromSingle<ParameterType, ResultType>(Func<ParameterType, ResultType> func, bool processesInParallel = false)
        {
            return new ParameterizedFacadeFunction<ParameterType, ResultType>(func, processesInParallel);
        }

        /// <summary>
        /// Creates a parameterized facade explicitly from an enumerable.
        /// </summary>
        /// <typeparam name="ParameterType">The type of the parameter.</typeparam>
        /// <typeparam name="ResultType">The type of the result.</typeparam>
        /// <param name="func">The Func that indicates how to process the request.</param>
        /// <returns>A new parameterized facade.</returns>
        public static ParameterizedFacadeFunction<ParameterType, ResultType> FromEnumerable<ParameterType, ResultType>(Func<IEnumerable<ParameterType>, IEnumerable<ResultType>> func)
        {
            return new ParameterizedFacadeFunction<ParameterType, ResultType>(func);
        }
    }
}

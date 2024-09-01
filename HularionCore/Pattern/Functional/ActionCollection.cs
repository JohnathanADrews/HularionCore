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
using System.Threading.Tasks;

namespace HularionCore.Pattern.Functional
{
    /// <summary>
    /// Contains a list of actions.
    /// </summary>
    public class ActionCollection
    {
        private List<Action> actions = new List<Action>();

        /// <summary>
        /// Adds an action as the first to call.
        /// </summary>
        /// <param name="action">The action to call.</param>
        public void AddPrefix(Action action)
        {
            lock (actions)
            {
                actions.Insert(0, action);
            }
        }

        /// <summary>
        /// Adds an action as the last to call.
        /// </summary>
        /// <param name="action">The action to call.</param>
        public void AddSuffix(Action action)
        {
            lock (actions)
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// Appends the actions in the order received.
        /// </summary>
        /// <param name="actions">The actions to add.</param>
        public void AddPrefixes(Action[] actions)
        {
            lock (this.actions)
            {
                for (int i = actions.Length - 1; i >= 0; i--)
                {
                    this.actions.Insert(0, actions[i]);
                }
            }
        }

        /// <summary>
        /// Prepends the actions in the order received.
        /// </summary>
        /// <param name="actions">The actions to add.</param>
        public void AddSuffixes(Action[] actions)
        {
            lock (this.actions)
            {
                for (int i = 0; i < actions.Length; i++)
                {
                    this.actions.Add(actions[i]);
                }
            }
        }

        /// <summary>
        /// Calls the actions in order.
        /// </summary>
        public void CallInOrder()
        {
            lock (actions)
            {
                foreach (var action in actions) { action(); }
            }
        }

        /// <summary>
        /// Calls the actions in reverse order.
        /// </summary>
        public void CallInReverseOrder()
        {
            lock (actions)
            {
                var reverse = new List<Action>(actions);
                reverse.Reverse();
                foreach (var action in reverse) { action(); }
            }
        }

        /// <summary>
        /// Calls the actions asynchronously.
        /// </summary>
        /// <param name="start">Iff true, starts the task before it is returned.</param>
        /// <param name="waitAll">Iff true, the task running the actions will wait for all of the actions to complete.</param>
        /// <returns>The task running the actions.</returns>
        public Task CallAsync(bool start = false, bool waitAll = true)
        {
            Task task = null;
            var tasks = new List<Task>();
            lock (actions)
            {
                task = new Task(() =>
                {
                    foreach (var action in actions)
                    {
                        var t = new Task(() => action());
                        t.Start();
                        tasks.Add(t);
                    }
                    if (waitAll)
                    {
                        Task.WaitAll(tasks.ToArray());
                    }
                });
            }
            if (start) { task.Start(); }
            return task;
        }
    }
}

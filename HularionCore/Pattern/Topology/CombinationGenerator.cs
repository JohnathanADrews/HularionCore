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

namespace HularionCore.Pattern.Topology
{
    /// <summary>
    /// Provides a standard implementation of a combination provider.
    /// </summary>
    /// <typeparam name="T">The type of the object to get the combinations for.</typeparam>
    public class CombinationGenerator<T> 
    {

        /// <summary>
        /// Generates all combinatons of the given size of the source items.
        /// </summary>
        /// <param name="size">The size of the combinations to generate.</param>
        /// <param name="source">The items to use to create the combinations.</param>
        /// <returns>All of the combinations of the given size.</returns>
        public IEnumerable<IEnumerable<T>> GenerateCombinations(int size, IEnumerable<T> source)
        {
            List<List<T>> combinations = new List<List<T>>();
            List<T> combination;
            if(size > source.Count())
            {
                return combinations;
            }
            T[] array = source.ToArray();

            Stack<int> positions = new Stack<int>();

            //Initialize the stack.
            for (int i = 0; i < size; i++)
            {
                positions.Push(i);
            }

            //Add the first combination.
            combination = new List<T>();
            foreach (int position in positions)
            {
                combination.Add(array[position]);
            }
            combinations.Add(combination);

            //Generate the new combinations.
            int index;
            while(true)
            {
                index = positions.Pop() + 1;
                //Decide how much of the stack to pop.
                //Let L = source length,
                //C = combination size,
                //I = the new index,
                //T = the size of the stack (after pop).
                //Required space = C - T
                //Available space = L - I
                //If required length > available length, then pop another or break.
                if (size - positions.Count > source.Count() - index)
                {
                    //In this case, there are no more valid combinations, so break;
                    if(positions.Count() == 0)
                    {
                        break;
                    }
                    continue;
                }
                //Fill up the stack with the values.
                while(positions.Count  < size)
                {
                    positions.Push(index++);
                }
                //Setup and add the new combination.
                combination = new List<T>();
                foreach(int position in positions)
                {
                    combination.Add(array[position]);
                }
                combinations.Add(combination);
            }

            return combinations;
        }
    }
}

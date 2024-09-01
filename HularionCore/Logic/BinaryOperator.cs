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

namespace HularionCore.Logic
{
    /// <summary>
    /// Represents all the binary-valued binary operators.
    /// </summary>
    public class BinaryOperator : IBinaryOperator
    {

        /// <summary>
        /// The name of this operator.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Evaluates the provided values against the implementing operator.
        /// </summary>
        /// <param name="p">The first (or left) operand.</param>
        /// <param name="q">The second (or right) operand.</param>
        /// <returns>The result of the evaluation.</returns>
        public bool Evaluate(IProvider<bool> p, IProvider<bool> q)
        {
            return operatorFunction(p.Provide(), q.Provide());
        }

        /// <summary>
        /// Evaluates the values against the implementing operator.
        /// </summary>
        /// <param name="p">The first (or left) operand.</param>
        /// <param name="q">The second (or right) operand.</param>
        /// <returns>The result of the evaluation.</returns>
        public bool Evaluate(bool a, bool b)
        {
            return this.operatorFunction(a, b);
        }

        /// <summary>
        /// A desciption of this operator.
        /// </summary>
        public string Description { get; private set; }

        public Func<bool, bool, bool> operatorFunction { get; private set; }

        /// <summary>
        /// true iff this operator is commutative.
        /// </summary>
        public bool IsCommutative { get; private set; }

        /// <summary>
        /// true iff this operator is associative.
        /// </summary>
        public bool IsAssociative { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">The unique identifier of the binary operator.</param>
        /// <param name="operatorDefinition"></param>
        private BinaryOperator(string name, Func<bool, bool, bool> operatorFunction)
        {
            this.Name = name;
            this.operatorFunction = operatorFunction;
            this.Description = String.Format("({0}) Signature: {1}, {2}, {3}, {4}", Name,
                operatorFunction(false, false) ? "1" : "0",
                operatorFunction(false, true) ? "1" : "0",
                operatorFunction(true, false) ? "1" : "0",
                operatorFunction(true, true) ? "1" : "0");
        }

        /// <summary>
        /// Gets the operator that has the provided output values.
        /// </summary>
        /// <param name="value0">The first output value.</param>
        /// <param name="value1">The second output value.</param>
        /// <param name="value2">The third output value.</param>
        /// <param name="value3">The fourth output value.</param>
        /// <returns>The operator that has the provided output values.</returns>
        public static BinaryOperator FromSignature(bool value0, bool value1, bool value2, bool value3)
        {
            if (value0)
            {
                if (value1)
                {
                    if (value2)
                    {
                        if (value3)
                        {
                            return BinaryOperator.PASS;
                        }
                        else
                        {
                            return BinaryOperator.NAND;
                        }
                    }
                    else
                    {
                        if (value3)
                        {
                            return BinaryOperator.NAORB;
                        }
                        else
                        {
                            return BinaryOperator.NA;
                        }
                    }
                }
                else
                {
                    if (value2)
                    {
                        if (value3)
                        {
                            return BinaryOperator.AORNB;
                        }
                        else
                        {
                            return BinaryOperator.NB;
                        }
                    }
                    else
                    {
                        if (value3)
                        {
                            return BinaryOperator.XNOR;
                        }
                        else
                        {
                            return BinaryOperator.NOR;
                        }
                    }
                }
            }
            else
            {
                if (value1)
                {
                    if (value2)
                    {
                        if (value3)
                        {
                            return BinaryOperator.OR;
                        }
                        else
                        {
                            return BinaryOperator.XOR;
                        }
                    }
                    else
                    {
                        if (value3)
                        {
                            return BinaryOperator.EB;
                        }
                        else
                        {
                            return BinaryOperator.NAB;
                        }
                    }
                }
                else
                {
                    if (value2)
                    {
                        if (value3)
                        {
                            return BinaryOperator.EA;
                        }
                        else
                        {
                            return BinaryOperator.ANB;
                        }
                    }
                    else
                    {
                        if (value3)
                        {
                            return BinaryOperator.AND;
                        }
                        else
                        {
                            return BinaryOperator.BLOCK;
                        }
                    }
                }
            }
        }
        

        /// Truth Table "Always 0"
        /// a b f
        /// 0 0 0
        /// 0 1 0
        /// 1 0 0
        /// 1 1 0
        public static BinaryOperator BLOCK = new BinaryOperator("BLOCK", (bool a, bool b) => { return false; }) { IsCommutative = true, IsAssociative = true };

        /// Truth Table "A and B"
        /// a b f
        /// 0 0 0
        /// 0 1 0
        /// 1 0 0
        /// 1 1 1
        public static BinaryOperator AND = new BinaryOperator("AND", (bool a, bool b) => { return a && b; }) { IsCommutative = true, IsAssociative = true };

        /// Truth Table "A and (not B)"
        /// a b f
        /// 0 0 0
        /// 0 1 0
        /// 1 0 1
        /// 1 1 0
        public static BinaryOperator ANB = new BinaryOperator("ANB", (bool a, bool b) => { return a && !b; }) { IsCommutative = false, IsAssociative = false };

        /// Truth Table "Exactly A"
        /// a b F
        /// 0 0 0
        /// 0 1 0
        /// 1 0 1
        /// 1 1 1
        public static BinaryOperator EA = new BinaryOperator("EA", (bool a, bool b) => { return a; }) { IsCommutative = false, IsAssociative = true };

        /// Truth Table "(not A) and B"
        /// a b f
        /// 0 0 0
        /// 0 1 1
        /// 1 0 0
        /// 1 1 0
        public static BinaryOperator NAB = new BinaryOperator("NAB", (bool a, bool b) => { return !a && b; }) { IsCommutative = false, IsAssociative = false };

        /// Truth Table "Exactly B"
        /// a b f
        /// 0 0 0
        /// 0 1 1
        /// 1 0 0
        /// 1 1 1
        public static BinaryOperator EB = new BinaryOperator("EB", (bool a, bool b) => { return b; }) { IsCommutative = false, IsAssociative = true };

        /// Truth Table "Exclusive OR -> (A and (not B)) or ((not B) and A)"
        /// a b f
        /// 0 0 0
        /// 0 1 1
        /// 1 0 1
        /// 1 1 0
        public static BinaryOperator XOR = new BinaryOperator("XOR", (bool a, bool b) => { return a ^ b; }) { IsCommutative = true, IsAssociative = true };

        /// Truth Table "A or B"
        /// a b f
        /// 0 0 0
        /// 0 1 1
        /// 1 0 1
        /// 1 1 1
        public static BinaryOperator OR = new BinaryOperator("OR", (bool a, bool b) => { return a || b; }) { IsCommutative = true, IsAssociative = true };

        /// Truth Table "not (A or B)"
        /// a b f
        /// 0 0 1
        /// 0 1 0
        /// 1 0 0
        /// 1 1 0
        public static BinaryOperator NOR = new BinaryOperator("NOR", (bool a, bool b) => { return !(a || b); }) { IsCommutative = true, IsAssociative = false };

        /// Truth Table Exclusive NOR -> (A and B) or ((not A) and (not B))"
        /// a b f
        /// 0 0 1
        /// 0 1 0
        /// 1 0 0
        /// 1 1 1
        public static BinaryOperator XNOR = new BinaryOperator("XNOR", (bool a, bool b) => { return !(a ^ b); }) { IsCommutative = true, IsAssociative = true };

        /// Truth Table "not B"
        /// a b f
        /// 0 0 1
        /// 0 1 0
        /// 1 0 1
        /// 1 1 0
        public static BinaryOperator NB = new BinaryOperator("NB", (bool a, bool b) => { return !b; }) { IsCommutative = false, IsAssociative = false };

        /// Truth Table "A or (not B)"
        /// a b f
        /// 0 0 1
        /// 0 1 0
        /// 1 0 1
        /// 1 1 1
        public static BinaryOperator AORNB = new BinaryOperator("AORNB", (bool a, bool b) => { return a || !b; }) { IsCommutative = false, IsAssociative = false };

        /// Truth Table (not A)
        /// a b f
        /// 0 0 1
        /// 0 1 1
        /// 1 0 0
        /// 1 1 0
        public static BinaryOperator NA = new BinaryOperator("NA", (bool a, bool b) => { return !a; }) { IsCommutative = false, IsAssociative = false };

        /// Truth Table "(not A) or B"
        /// a b f
        /// 0 0 1
        /// 0 1 1
        /// 1 0 0
        /// 1 1 1
        public static BinaryOperator NAORB = new BinaryOperator("NAORB", (bool a, bool b) => { return !a || b; }) { IsCommutative = false, IsAssociative = false };

        /// Truth Table "not (A and B)"
        /// a b f
        /// 0 0 1
        /// 0 1 1
        /// 1 0 1
        /// 1 1 0
        public static BinaryOperator NAND = new BinaryOperator("NAND", (bool a, bool b) => { return !(a && b); }) { IsCommutative = true, IsAssociative = false };

        /// Truth Table "Always 1"
        /// a b f
        /// 0 0 1
        /// 0 1 1
        /// 1 0 1
        /// 1 1 1
        public static BinaryOperator PASS = new BinaryOperator("PASS", (bool a, bool b) => { return true; }) { IsCommutative = true, IsAssociative = true };


        public override bool Equals(object obj)
        {
          //  if(obj.GetType() != typeof(BinaryOperator)) { return false; }
            return Name == ((BinaryOperator)obj).Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        /// The equals comparison.
        /// </summary>
        /// <param name="operator1">The first operator.</param>
        /// <param name="operator2">The second operator</param>
        /// <returns>True iff the operators are equal.</returns>
        public static bool operator ==(BinaryOperator operator1, BinaryOperator operator2)
        {
            return Equals(operator1, operator2);
        }

        /// <summary>
        /// The inequality comparison.
        /// </summary>
        /// <param name="operator1">The first operator.</param>
        /// <param name="operator2">The second operator</param>
        /// <returns>True iff the operators are not equal.</returns>
        public static bool operator !=(BinaryOperator operator1, BinaryOperator operator2)
        {
            return !Equals(operator1, operator2);
        }


        public override string ToString()
        {
            return Description;
        }

    }


}

using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	/// <summary>
	/// This node is an operator in the equation.
	/// </summary>
	public class OperatorNode : BaseNode
	{
		#region Members
		
		#endregion Members

		#region Properties

		public char Operator
		{
			set
			{
				//what operator is it?
				switch (value)
				{
					case '^':
					{
						OrderOfOperationsValue = PemdasValue.Exponent;
					}
						break;
					case '*':
					{
						OrderOfOperationsValue = PemdasValue.Multiplication;
					}
						break;
					case '/':
					{
						OrderOfOperationsValue = PemdasValue.Division;
					}
						break;
					case '+':
					{
						OrderOfOperationsValue = PemdasValue.Addition;
					}
						break;
					case '-':
					{
						OrderOfOperationsValue = PemdasValue.Subtraction;
					}
						break;
					default:
					{
						throw new FormatException("invalid operator text: " + value);
					}
				}
			}
		}

		#endregion //Properties
		
		#region Methods
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.FunctionNode"/> class.
		/// </summary>
		public OperatorNode()
		{
			//default to invalid, this will be set by the Parse function
			OrderOfOperationsValue = PemdasValue.Invalid;
		}
		
		/// <summary>
		/// Parse the specified tokenList and curIndex.
		/// overloaded by child types to do there own specific parsing.
		/// </summary>
		/// <param name="tokenList">Token list.</param>
		/// <param name="curIndex">Current index.</param>
		/// <param name="owner">the equation that this node is part of.  required to pull function delegates out of the dictionary</param>
		protected override void ParseToken(List<Token> tokenList, ref int curIndex, Equation owner)
		{
			Debug.Assert(null != tokenList);
			Debug.Assert(null != owner);
			Debug.Assert(curIndex < tokenList.Count);

			//make sure the token text is the correct length
			if (tokenList[curIndex].TokenText.Length != 1)
			{
				throw new FormatException("operator text length can only be one character, was given " + tokenList[curIndex].TokenText);
			}
			
			//get the operator
			Operator = tokenList[curIndex].TokenText[0];
			
			//increment the current index since we consumed the operator token
			curIndex++;
		}

		/// <summary>
		/// Solve the equation!
		/// This method recurses into the whole tree and returns a result from the equation.
		/// </summary>
		/// <param name="paramCallback">Parameter callback that will be used to get teh values of parameter nodes.</param>
		/// <returns>The solution of this node and all its subnodes!</returns>
		public override float Solve(ParamDelegate paramCallback)
		{
			//make sure this node is set up correctly
			Debug.Assert(null != Prev);
			Debug.Assert(null != Next);

			//Solve the sub nodes!
			float prevResult = Prev.Solve(paramCallback);
			float nextResult = Next.Solve(paramCallback);

			//what kind of operator do we got?
			switch (OrderOfOperationsValue)
			{
				case PemdasValue.Exponent:
				{
					return (float)Math.Pow(prevResult, nextResult);
				}
				case PemdasValue.Multiplication:
				{
					return prevResult * nextResult;
				}
				case PemdasValue.Division:
				{
					//TODO: can hit divide by zero exception here
					return prevResult / nextResult;
				}
				case PemdasValue.Addition:
				{
					return prevResult + nextResult;
				}
				case PemdasValue.Subtraction:
				{
					return prevResult - nextResult;
				}
				default:
				{
					throw new NotSupportedException("found a weirdo thing in an equation node?");
				}
			}
		}
		
		#endregion Methods
	}
}
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	public class EquationNode : BaseNode
	{
		#region Members

		/// <summary>
		/// An equation node holds an entire sub-equation that is contained in parenthesis
		/// </summary>
		/// <value>The sub equation.</value>
		private BaseNode SubEquation { get; set; }
		
		#endregion Members
		
		#region Methods
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.FunctionNode"/> class.
		/// </summary>
		public EquationNode()
		{
			OrderOfOperationsValue = PemdasValue.Value;
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

			//parse the equation into our subnode
			SubEquation = BaseNode.Parse(tokenList, ref curIndex, owner);

			//if some smart ass types in () the parse method wouldve returned null.  
			//it should evaluate to 0 in that case, so create a number node and set the value.
			if (null == SubEquation)
			{
				NumberNode fakeNode = new NumberNode();
				fakeNode.NumberValue = 0.0f;
				SubEquation = fakeNode;
			}
			Debug.Assert(null != SubEquation);

			//treeify the subequation so we can solve it
			SubEquation = SubEquation.Treeify();
			Debug.Assert(null != SubEquation);
		}

		/// <summary>
		/// This method gets called when the token parser encounters a minus sign in front of a value.
		/// If the next token is a number, it will be changed to a negative number.
		/// If the next token is a funcion, param, or equation, an equation will be generated that multiplies the result by -1
		/// </summary>
		/// <returns>The negative token.</returns>
		/// <param name="tokenList">Token list.</param>
		/// <param name="curIndex">Current index.</param>
		/// <param name="owner">Owner.</param>
		public static BaseNode ParseNegativeToken(List<Token> tokenList, ref int curIndex, Equation owner)
		{
			//verify that this is not the last token
			if (curIndex >= (tokenList.Count - 1))
			{
				throw new FormatException("Can't end an equation with an operator");
			}
			
			//check that the token is a minus sign
			if ("-" != tokenList[curIndex].TokenText)
			{
				throw new FormatException("Expected a value, but found an invalid operator instead");
			}
			
			//skip past the minus sign so we can get to the next token
			curIndex++;
			
			//create a number node, parse the next token into it
			BaseNode valueNode = BaseNode.ParseValueNode(tokenList, ref curIndex, owner);
			Debug.Assert(null != valueNode);

			//what did we get back?
			if (valueNode is NumberNode)
			{
				//the next node is a number, multiply it by minus one
				NumberNode myNumberNode = valueNode as NumberNode;
				myNumberNode.NumberValue *= -1.0f;
			}
			else
			{
				//ok the node was a function, param, or equation

				//create another equation to multiply that resdult by -1
				NumberNode negativeOne = new NumberNode();
				negativeOne.NumberValue = -1.0f;
				OperatorNode multiplyNode = new OperatorNode();
				multiplyNode.Operator = '*';

				//string it all together
				negativeOne.AppendNextNode(multiplyNode);
				multiplyNode.AppendNextNode(valueNode);

				//put that into an equation node and treeify it
				EquationNode myEquationNode = new EquationNode();
				myEquationNode.SubEquation = negativeOne.Treeify();
				Debug.Assert(null != myEquationNode.SubEquation);

				//set our result to the whole equation
				valueNode = myEquationNode;
			}
			
			//return it as the result
			return valueNode;
		}

		/// <summary>
		/// Solve the equation!
		/// This method recurses into the whole tree and returns a result from the equation.
		/// </summary>
		/// <param name="paramCallback">Parameter callback that will be used to get teh values of parameter nodes.</param>
		/// <returns>The solution of this node and all its subnodes!</returns>
		public override float Solve(ParamDelegate paramCallback)
		{
			//Return the sub equation solver
			Debug.Assert(null != SubEquation);
			return SubEquation.Solve(paramCallback);
		}
		
		#endregion Methods
	}
}
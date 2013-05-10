using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	public class FunctionNode : BaseNode
	{
		#region Members

		/// <summary>
		/// the 4 cahracter text from the script of the function
		/// </summary>
		/// <value>The name of the function.</value>
		private string FunctionName { get; set; }
		
		/// <summary>
		/// Gets or sets the function delegate
		/// </summary>
		/// <value>The index.</value>
		private FunctionDelegate MyFunction { get; set; }
		
		#endregion Members
		
		#region Methods
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.FunctionNode"/> class.
		/// </summary>
		public FunctionNode()
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
			
			//get the function name
			FunctionName = tokenList[curIndex].TokenText;

			//check if the function is in the equation dictionary
			if (!owner.FunctionDictionary.ContainsKey(FunctionName))
			{
				throw new FormatException("Unknown function call: " + FunctionName);
			}

			//set the function delegate
			MyFunction = owner.FunctionDictionary[FunctionName];
			
			//increment the current index since we consumed the function name token
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
			//Return the function we found in the parser
			Debug.Assert(null != MyFunction);
			return MyFunction();
		}
		
		#endregion Methods
	}
}
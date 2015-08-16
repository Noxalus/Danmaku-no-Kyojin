using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	/// <summary>
	/// This node evaluates to a paramter that is passed into the equation at solve time
	/// </summary>
	public class ParamNode : BaseNode
	{
		#region Members
		
		/// <summary>
		/// Gets or sets the parameter index
		/// </summary>
		/// <value>The index.</value>
		private int ParamIndex { get; set; }
		
		#endregion Members
		
		#region Methods
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.ParamNode"/> class.
		/// </summary>
		public ParamNode()
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
			
			//get the number out of the list
			try
			{
				ParamIndex = Convert.ToInt32(tokenList[curIndex].TokenText);
			}
			catch
			{
				throw new FormatException("Could not parse \"" + tokenList[curIndex].TokenText.ToString() + "\" into a parameter index.");
			}

			//double check that the index is valid
			if ((ParamIndex <= 0) || (ParamIndex > 9))
			{
				throw new FormatException("Parameter index must be between 1 - 9");
			}
			
			//increment the current index since we consumed the parameter index token
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
			//get the parameter value.
			Debug.Assert(null != paramCallback);
			return paramCallback(ParamIndex);
		}
		
		#endregion Methods
	}
}
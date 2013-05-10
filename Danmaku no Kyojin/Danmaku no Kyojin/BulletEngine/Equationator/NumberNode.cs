using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Globalization;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	/// <summary>
	/// This a node in the equation that holds a number.
	/// </summary>
	public class NumberNode : BaseNode
	{
		#region Members

		/// <summary>
		/// The actual number value of this node
		/// </summary>
		/// <value>The number value.</value>
		private float _num;

		/// <summary>
		/// Gets or sets the number.
		/// </summary>
		/// <value>The number.</value>
		public float NumberValue 
		{ 
			get
			{
				return _num;
			}
			set
			{
				_num = value;
			}
		}

		#endregion Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.NumberNode"/> class.
		/// </summary>
		public NumberNode()
		{
			OrderOfOperationsValue = PemdasValue.Value;
			_num = 0.0f;
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
            if (!float.TryParse(tokenList[curIndex].TokenText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _num))
			{
                throw new FormatException("Could not parse \"" + tokenList[curIndex].TokenText.ToString() + "\" into a number.");
			}

			//increment the current index since we consumed the number token
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
			//Return our number
			return NumberValue;
		}

		#endregion Methods
	}
}
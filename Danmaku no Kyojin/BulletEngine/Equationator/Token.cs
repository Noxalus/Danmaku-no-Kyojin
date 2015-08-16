using System;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	/// <summary>
	/// This is a single text token from an equation.
	/// The first step to compiling an equation is breaking it up into a list tokens and determining what is in those tokens.
	/// </summary>
	public class Token
	{
		#region Members

		/// <summary>
		/// Gets the token text.
		/// </summary>
		/// <value>The token text.</value>
		public string TokenText { get; private set; }

		/// <summary>
		/// Gets the type of token.
		/// </summary>
		/// <value>The type of token.</value>
		public TokenType TypeOfToken { get; private set; }

		#endregion Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.Token"/> class.
		/// </summary>
		/// <param name="myText">My text.</param>
		/// <param name="myType">My type.</param>
		public Token(string myText, TokenType myType)
		{
			TokenText = myText;
			TypeOfToken = myType;
		}

		#endregion Methods
	}
}


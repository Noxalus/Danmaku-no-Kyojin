using System.Collections.Generic;
using System;
using System.Text;
using System.Diagnostics;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	/// <summary>
	/// This is the main class of teh Equationator. 
	/// It contains one entire equation.  
	/// Usage is to create one of these, add all the delegate methods, parse the equation string, and then call teh Solve function whenever need a result from it.
	/// A good idea would be to subclass this and have the child class set up all the delegate methods in it's constructor.
	/// </summary>
	public class Equation
	{
		#region Members

		/// <summary>
		/// The text equation that this dude parsed.
		/// </summary>
		public string TextEquation { get; private set; }

		/// <summary>
		/// This is the root node of the equation.  This is set in the Parse method.
		/// </summary>
		private BaseNode RootNode { get; set; }

		/// <summary>
		/// A list of all the names and functions that can be used in teh equation grammar of this dude.
		/// </summary>
		public Dictionary<string, FunctionDelegate> FunctionDictionary { get; private set; }

		#endregion Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.Equation"/> class.
		/// </summary>
		public Equation()
		{
			FunctionDictionary = new Dictionary<string, FunctionDelegate>();
		}

		/// <summary>
		/// Adds a function to the grammar dictionaary so that it can be used in equations.
		/// </summary>
		/// <param name="FunctionText">Function text. Must be 4 characters, no numerals</param>
		/// <param name="callbackMethod">Callback method that will be called when $XXXX is encountered in an equation</param>
		/// <exception cref="FormatException">thrown when the fucntionText is incorrect format</exception>
		public void AddFunction(string functionText, FunctionDelegate callbackMethod)
		{
			if (4 != functionText.Length)
			{
				//string length of a function text must be 4 cahracters, no numbers
				//This makes it easy to parse when we find $xxxx (that means it's a function)
				throw new FormatException("The functionText parameter must be exactly four characters in length.");
			}

			//Store the thing in the dictionary
			FunctionDictionary.Add(functionText, callbackMethod);
		}

		/// <summary>
		/// Parse the specified equationText.
		/// </summary>
		/// <param name="equationText">Equation text.</param>
		public void Parse(string equationText)
		{
			//grab the equation text
			TextEquation = equationText;

			//straight up tokenize the equation: operators, numbers, parens, functions, params
			List<Token> tokenList = Tokenize(equationText);

			//sort out those tokens into a linked list of equation nodes
			int index = 0;
			BaseNode listRootNode = BaseNode.Parse(tokenList, ref index, this);

			//take that linked list and bend it into a binary tree.  Grab the root node
			RootNode = listRootNode.Treeify();
		}

		/// <summary>
		/// Get a result from the equation
		/// </summary>
		/// <param name="paramCallback">This is a callback function to get the value of params to pass to this equation</param>
		public float Solve(ParamDelegate paramCallback)
		{
			Debug.Assert(null != RootNode);
			return RootNode.Solve(paramCallback);
		}

		/// <summary>
		/// Tokenize the specified equationText.
		/// </summary>
		/// <param name="equationText">Equation text.</param>
		/// <returns>A list of tokens that were contained in the equation text.</returns>
		private List<Token> Tokenize(string equationText)
		{
			//The list that will hold all our tokens.
			List<Token> tokenList = new List<Token>();

			//Walk through the text and try to parse it out into an expression
			StringBuilder word = new StringBuilder();
			for (int i = 0; i < equationText.Length; i++)
			{
				//First check if we are reading in a number
				if (('0' <= equationText[i] && equationText[i] <= '9') || equationText[i] == '.')
				{
					//Add the digit/decimal to the end of the number
					word.Append(equationText[i]);
					
					//If we haven't reached the end of the text, keep reading
					if (i < equationText.Length - 1)
					{
						continue;
					}
				}
				
				//If we have a string in there, it has to be a number that has been parsed up above
				if (!string.IsNullOrEmpty(word.ToString()))
				{
					tokenList.Add(new Token(word.ToString(), TokenType.Number));
					word.Clear();
				}
				
				//We aren't reading a string, and if we had a number it was already stored up above... check what the current character is
				if (equationText[i] == '$')
				{
					//we found a variable, check if it is a param or a function call
					if (equationText[i + 1] >= '0' && equationText[i + 1] <= '9')
					{
						//We have a param value, parse it out and store in the list of values
						tokenList.Add(new Token(equationText[i + 1].ToString(), TokenType.Param));
						
						//since we consumed the $ followed by param number, increment the index by 1
						i++;
					}
					else
					{
						//skip over the dollar sign
						i++;

						//check if the 4 cahracter substring is stored in our grammar dictionary
						string subString = equationText.Substring(i, 4);

						if (FunctionDictionary.ContainsKey(subString))
						{
							//We found a matching function call in the dictionary
							tokenList.Add(new Token(subString, TokenType.Function));

							//since we consumed the $ followed by 4 characters, increment the index by 3 and the for loop will increment the last one
							i += 3;
						}
						else
						{
							//error: there was a $something that wasn't a param and wasn't in our function dictionary
							throw new FormatException("Equation text contained $XXXX that was not found in the grammar dictionary");
						}
					}
				}
				else if (equationText[i] == '(')
				{
					//we found an open paren!
					tokenList.Add(new Token(equationText[i].ToString(), TokenType.OpenParen));
				}
				else if (equationText[i] == ')')
				{
					//we found a close paren!
					tokenList.Add(new Token(equationText[i].ToString(), TokenType.CloseParen));
				}
				else if (equationText[i] == '*' || 
				         equationText[i] == '/' || 
				         equationText[i] == '+' || 
				         equationText[i] == '-' ||
				         equationText[i] == '^')
				{
					//We found an operator value...
					tokenList.Add(new Token(equationText[i].ToString(), TokenType.Operator));
				}
			}

			//ok, this should contain our whole entire token list
			return tokenList;
		}

		#endregion Methods
	}
}
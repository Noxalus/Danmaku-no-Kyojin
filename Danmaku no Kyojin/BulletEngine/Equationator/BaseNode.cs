using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Danmaku_no_Kyojin.BulletEngine.Equationator
{
	/// <summary>
	/// This is a single node of the equation.
	/// All the other node types inherit from this one.
	/// </summary>
	public abstract class BaseNode
	{
		#region Members
			
		/// <summary>
		/// Gets or sets the previous node.
		/// </summary>
		/// <value>The previous.</value>
		protected BaseNode Prev { get; set; }
			
		/// <summary>
		/// Gets or sets the next node.
		/// </summary>
		/// <value>The next.</value>
		protected BaseNode Next { get; set; }

		/// <summary>
		/// The value of this node for order of operations.
		/// This is set in all teh chlid nodes.
		/// </summary>
		/// <value>The pembas value.</value>
		protected PemdasValue OrderOfOperationsValue { get; set; }
			
		#endregion Members
		
		#region Methods

		#region Constructor
			
		/// <summary>
		/// Initializes a new instance of the <see cref="Equationator.EquationNode"/> class.
		/// </summary>
		/// <param name="prev">Previous node in the list, or null if this is head</param>
		/// <param name="next">Next node in the list, or null if this is tail.</param>
		public BaseNode()
		{
			this.Prev = null;
			this.Next = null;
		}

		#endregion //Constructor

		#region Linked List Functionality
			
		/// <summary>
		/// Make this node into a head node.
		/// </summary>
		protected void MakeHead()
		{
			//note that if there is a prev node, it has to take care of it's own pointers!
			Prev = null;
		}
			
		/// <summary>
		/// Makes this node into a tail node.
		/// </summary>
		protected void MakeTail()
		{
			//note that if there is a next node, it has to take care of it's own pointers!
			Next = null;
		}
			
		/// <summary>
		/// Appends the next node.
		/// </summary>
		/// <param name="nextNode">Next node.</param>
		public void AppendNextNode(BaseNode nextNode)
		{
			Debug.Assert(null != nextNode);
			nextNode.Prev = this;
			this.Next = nextNode;
		}

		/// <summary>
		/// Add the ability for any node in a list to get the head node of the list.
		/// </summary>
		/// <returns>The head.</returns>
		public BaseNode GetHead()
		{
			//If there is a previous node, recurse into it to get the head node.
			if (null != Prev)
			{
				return Prev.GetHead();
			}
			else
			{
				//If there is no prev node, this dude is the head
				return this;
			}
		}

		#endregion //Linked List Functionality

		#region Token Parsing Functionality

		/// <summary>
		/// Parse a list of tokens into a linked list of equation nodes.
		/// This will sort it out into a flat equation
		/// </summary>
		/// <param name="tokenList">Token list.</param>
		/// <param name="curIndex">Current index. When this function exits, will be incremented to the past any tokens consumed by this method</param>
		/// <param name="owner">the equation that this node is part of.  required to pull function delegates out of the dictionary</param>
		/// <returns>A basenode pointing at the head of a linked list parsed by this method</returns>
		static public BaseNode Parse(List<Token> tokenList, ref int curIndex, Equation owner)
		{
			Debug.Assert(null != tokenList);
			Debug.Assert(null != owner);
			Debug.Assert(curIndex < tokenList.Count);

			//first get a value, which will be a number, function, param, or equation node
			BaseNode myNumNode = BaseNode.ParseValueNode(tokenList, ref curIndex, owner);
			Debug.Assert(null != myNumNode);

			//if there are any tokens left, get an operator
			if (curIndex < tokenList.Count)
			{
				BaseNode myOperNode = BaseNode.ParseOperNode(tokenList, ref curIndex, owner);

				if (null != myOperNode)
				{
					//add that node to the end of the list
					myNumNode.AppendNextNode(myOperNode);

					//If it was able to pull an operator out, there has to be a number after it.
					if (curIndex >= tokenList.Count)
					{
						throw new FormatException("Can't end an equation with an operator.");
					}

					//Recurse into the parse function and sort out the rest of the tokens
					BaseNode nextNode = BaseNode.Parse(tokenList, ref curIndex, owner);
					Debug.Assert(null != nextNode);
					
					//add that node to the end of the list
					myOperNode.AppendNextNode(nextNode);
				}
			}

			//return the head node that I found
			return myNumNode;
		}

		/// <summary>
		/// Given a list of tokens and the index, get a node based on whatever is at that index
		/// </summary>
		/// <returns>The value node, will be a number, function, param, or equation node</returns>
		/// <param name="tokenList">Token list.</param>
		/// <param name="curIndex">Current index.</param>
		/// <param name="owner">the equation that this node is part of.  required to pull function delegates out of the dictionary</param>
		static protected BaseNode ParseValueNode(List<Token> tokenList, ref int curIndex, Equation owner)
		{
			Debug.Assert(null != tokenList);
			Debug.Assert(null != owner);
			Debug.Assert(curIndex < tokenList.Count);

			//what kind of token do I have at that index?
			switch (tokenList[curIndex].TypeOfToken)
			{
				case TokenType.Number:
				{
					//awesome, that's nice and easy... just shove the text into a node as a number

					//create the number node
					NumberNode valueNode = new NumberNode();

					//parse the text into the number node
					valueNode.ParseToken(tokenList, ref curIndex, owner);

					//return the number node as our result
					return valueNode;
				}

				case TokenType.Param:
				{
					//also not bad, grab the text as a parameter index and put in a node

					//create the param node
					ParamNode valueNode = new ParamNode();

					//parse the parameter index into the node
					valueNode.ParseToken(tokenList, ref curIndex, owner);

					//return it as our result
					return valueNode;
				}

				case TokenType.Function:
				{
					//hmmm... need to get the delegate and put in a node?

					//create the function node
					FunctionNode valueNode = new FunctionNode();
					
					//parse the function delegate into the node
					valueNode.ParseToken(tokenList, ref curIndex, owner);
					
					//return it as our result
					return valueNode;
				}

				case TokenType.OpenParen:
				{
					//ok don't panic... 

					//verify that this is not the last token
					if (curIndex >= (tokenList.Count - 1))
					{
						throw new FormatException("Can't end an equation with an open paranthesis");
					}

					//move past this token, cuz nothing else to do with it
					curIndex++;

					//starting at the next token, start an equation node
					EquationNode valueNode = new EquationNode();

					//start parsing into the equation node
					valueNode.ParseToken(tokenList, ref curIndex, owner);

					//return it as the result
					return valueNode;
				}

				case TokenType.Operator:
				{
					//whoa, how did an operator get in here?  it better be a minus sign
					return EquationNode.ParseNegativeToken(tokenList, ref curIndex, owner);
				}

				default:
				{
					//should just be close paren nodes in here, which we should never get
					throw new FormatException("Expected a \"value\" token, but got a " + tokenList[curIndex].TypeOfToken.ToString());
				}
			}
		}

		/// <summary>
		/// Given a list of tokens and the index, get an operator node based on whatever is at that index.
		/// </summary>
		/// <returns>The oper node, or null if it hit the end of the equation.</returns>
		/// <param name="tokenList">Token list.</param>
		/// <param name="curIndex">Current index.</param>
		/// <param name="owner">the equation that this node is part of.  required to pull function delegates out of the dictionary</param>
		static protected BaseNode ParseOperNode(List<Token> tokenList, ref int curIndex, Equation owner)
		{
			Debug.Assert(null != tokenList);
			Debug.Assert(null != owner);
			Debug.Assert(curIndex < tokenList.Count);

			//what kind of token do I have at that index?
			switch (tokenList[curIndex].TypeOfToken)
			{
				case TokenType.Operator:
				{
					//ok create an operator node
					OperatorNode operNode = new OperatorNode();
					
					//parse into that node
					operNode.ParseToken(tokenList, ref curIndex, owner);
					
					//return the thing
					return operNode;
				}

				case TokenType.CloseParen:
				{
					//close paren, just eat it and return null.  It means this equation is finished parsing
					curIndex++;
					return null;
				}

				default:
				{
					//should just be close paren nodes in here, which we should never get
					throw new FormatException("Expected a \"operator\" token, but got a " + tokenList[curIndex].TypeOfToken.ToString());
				}
			}
			
		}

		/// <summary>
		/// Parse the specified tokenList and curIndex.
		/// overloaded by child types to do there own specific parsing.
		/// </summary>
		/// <param name="tokenList">Token list.</param>
		/// <param name="curIndex">Current index.</param>
		/// <param name="owner">the equation that this node is part of.  required to pull function delegates out of the dictionary</param>
		protected abstract void ParseToken(List<Token> tokenList, ref int curIndex, Equation owner);

		#endregion //Token Parsing Functionality

		#region Treeifying Functionality

		/// <summary>
		/// This method takes a node from a linked list, and folds it into a binary tree.
		/// The root node will have the highest pemdas value and the tree will be solved breadth first, ensuring that the root node is solved last.
		/// </summary>
		public BaseNode Treeify()
		{
			//This method should only ever be called on head nodes!
			Debug.Assert(null == Prev);

			//If this is a leaf node, there is nothing to do here!
			if (null == Next)
			{
				return this;
			}

			//find the node with the highest pemdas value (or the first subtraction node, those are always highest)
			BaseNode RootNode = GetHighestPemdas();

			//by this point nodes are either leaf or binary nodes with a node at each end
			Debug.Assert(null != RootNode.Prev);
			Debug.Assert(null != RootNode.Next);

			//set the next node to be the head of it's own list
			RootNode.Next.MakeHead();

			//set the prev node to be the tail of it's own list
			RootNode.Prev.MakeTail();

			//set the prev of our root node to the head of the prev list
			RootNode.Prev = RootNode.Prev.GetHead();

			//set the prev node to the treeifyication result of the "previous" linked list
			RootNode.Prev = RootNode.Prev.Treeify();

			//set the next node to treeification result of the "next" linked list
			RootNode.Next = RootNode.Next.Treeify();

			//that's it, the whole equation below the root node is treeified now
			return RootNode;
		}

		/// <summary>
		/// Gets the highest pemdas.
		/// </summary>
		/// <returns>The highest pemdas.</returns>
		private BaseNode GetHighestPemdas()
		{
			Debug.Assert(null != Next);
			BaseNode RootNode = this;
			for  (BaseNode iter = Next; iter != null; iter = iter.Next)
			{
				//if we found a subtraction node, that is the highest value
				if (PemdasValue.Subtraction ==  RootNode.OrderOfOperationsValue)
				{
					break;
				}
				else if (iter.OrderOfOperationsValue > RootNode.OrderOfOperationsValue)
				{
					//The next node has a higher value than the current champion
					RootNode = iter;
				}
			}

			//ok, return the node we found with the highest value.
			return RootNode;
		}

		#endregion //Treeifying Functionality

		#region Solve Functionality

		/// <summary>
		/// Solve the equation!
		/// This method recurses into the whole tree and returns a result from the equation.
		/// </summary>
		/// <param name="paramCallback">Parameter callback that will be used to get teh values of parameter nodes.</param>
		/// <returns>The solution of this node and all its subnodes!</returns>
		public abstract float Solve(ParamDelegate paramCallback);

		#endregion //Solve Functionality

		#endregion Methods
	}
}

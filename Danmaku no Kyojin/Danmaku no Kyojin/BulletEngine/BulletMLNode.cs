using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This is a single node from a BulletML document.
	/// </summary>
	public class BulletMLNode
	{
		#region Members

		/// <summary>
		/// The XML node name of this item
		/// </summary>
		public ENodeName Name { get; private set; }

		/// <summary>
		/// The type modifie of this node... like is it a sequence, or whatver
		/// idunno, this is really poorly thought out on the part of Kento Cho
		/// </summary>
		public ENodeType NodeType { get; private set; }

		/// <summary>
		/// The label of this node
		/// This can be used by other nodes to reference this node
		/// </summary>
		public string Label { get; private set; }

		/// <summary>
		/// An equation used to get a value of this node.
		/// </summary>
		/// <value>The node value.</value>
		private BulletMLEquation NodeEquation = new BulletMLEquation();

		/// <summary>
		/// A list of all the child nodes for this dude
		/// </summary>
		public List<BulletMLNode> ChildNodes = new List<BulletMLNode>();

		/// <summary>
		/// pointer to the parent node of this dude
		/// </summary>
		public BulletMLNode Parent { get; private set; }

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletMLNode"/> class.
		/// </summary>
		public BulletMLNode()
		{
			Name = ENodeName.bulletml;
			NodeType = ENodeType.absolute;
		}

		/// <summary>
		/// Convert a string to it's ENodeType enum equivalent
		/// </summary>
		/// <returns>ENodeType: the nuem value of that string</returns>
		/// <param name="str">The string to convert to an enum</param>
		private static ENodeType StringToType(string str)
		{
			//make sure there is something there
			if (string.IsNullOrEmpty(str))
			{
				return ENodeType.none;
			}
			else
			{
				return (ENodeType)Enum.Parse(typeof(ENodeType), str);
			}
		}
		
		/// <summary>
		/// Convert a string to it's ENodeName enum equivalent
		/// </summary>
		/// <returns>ENodeName: the nuem value of that string</returns>
		/// <param name="str">The string to convert to an enum</param>
		private static ENodeName StringToName(string str)
		{
			return (ENodeName)Enum.Parse(typeof(ENodeName), str);
		}

		/// <summary>
		/// Gets the root node.
		/// </summary>
		/// <returns>The root node.</returns>
		public BulletMLNode GetRootNode()
		{
			//recurse up until we get to the root node
			if (null != Parent)
			{
				return Parent.GetRootNode();
			}

			//if it gets here, there is no parent node and this is the root.
			return this;
		}

		/// <summary>
		/// Find a node of a specific type and label
		/// Recurse into the xml tree until we find it!
		/// </summary>
		/// <returns>The label node.</returns>
		/// <param name="label">Label of the node we are looking for</param>
		/// <param name="name">name of the node we are looking for</param>
		public BulletMLNode FindLabelNode(string strLabel, ENodeName eName)
		{
			//this uses breadth first search, since labelled nodes are usually top level

			//Check if any of our child nodes match the request
			for (int i = 0; i < ChildNodes.Count; i++)
			{
				if ((eName == ChildNodes[i].Name) && (strLabel == ChildNodes[i].Label))
				{
					return ChildNodes[i];
				}
			}

			//recurse into the child nodes and see if we find any matches
			for (int i = 0; i < ChildNodes.Count; i++)
			{
				BulletMLNode foundNode = ChildNodes[i].FindLabelNode(strLabel, eName);
				if (null != foundNode)
				{
					return foundNode;
				}
			}

			//didnt find a BulletMLNode with that name :(
			return null;
		}
		
		/// <summary>
		/// Parse the specified bulletNodeElement.
		/// Read all the data from the xml node into this dude.
		/// </summary>
		/// <param name="bulletNodeElement">Bullet node element.</param>
		public bool Parse(XmlNode bulletNodeElement, BulletMLNode parentNode)
		{
			Debug.Assert(null != bulletNodeElement);

			//grab the parent node
			Parent = parentNode;

			//get the node type
			Name = BulletMLNode.StringToName(bulletNodeElement.Name);

			//Parse all our attributes
			XmlNamedNodeMap mapAttributes = bulletNodeElement.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;

				if ("type" == strName)
				{
					//skip the type attribute in top level nodes
					if (ENodeName.bulletml == Name)
					{
						continue;
					}

					//get the bullet node type
					NodeType = BulletMLNode.StringToType(strValue);
				}
				else if ("label" == strName)
				{
					//label is just a text value
					Label = strValue;
				}
			}

			//parse all the child nodes
			if (bulletNodeElement.HasChildNodes)
			{
				for (XmlNode childNode = bulletNodeElement.FirstChild;
					     null != childNode;
					     childNode = childNode.NextSibling)
				{
					//if the child node is a text node, parse it into this dude
					if (XmlNodeType.Text == childNode.NodeType)
					{
						//Get the text of the child xml node, but store it in THIS bullet node
						NodeEquation.Parse(childNode.Value);
						continue;
					}

					//create a new node
					BulletMLNode childBulletNode = new BulletMLNode();

					//read in the node
					if (!childBulletNode.Parse(childNode, this))
					{
						return false;
					}

					//store the node
					ChildNodes.Add(childBulletNode);
				}
			}

			return true;
		}

		/// <summary>
		/// Find a parent node of the specified node type
		/// </summary>
		/// <returns>The first parent node of that type, null if none found</returns>
		/// <param name="nodeType">Node type to find.</param>
		public BulletMLNode FindParentNode(ENodeName nodeType)
		{
			//first check if we have a parent node
			if (null == Parent)
			{
				return null;
			}
			else if (nodeType == Parent.Name)
			{
				//Our parent matches the query, reutrn it!
				return Parent;
			}
			else
			{
				//recurse into parent nodes to check grandparents, etc.
				return Parent.FindParentNode(nodeType);
			}
		}

		//TODO: sort all these shitty functions out

		public float GetChildValue(ENodeName name, BulletMLTask task)
		{
			foreach (BulletMLNode tree in ChildNodes)
			{
				if (tree.Name == name)
				{
					return tree.GetValue(task);
				}
			}
			return 0.0f;
		}
		
		public BulletMLNode GetChild(ENodeName name)
		{
			foreach (BulletMLNode node in ChildNodes)
			{
				if (node.Name == name)
				{
					return node;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the value of this node for a specific instance of a task.
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="task">Task.</param>
		public float GetValue(BulletMLTask task)
		{
			//send to the equation for an answer
			return NodeEquation.Solve(task.GetParamValue);
		}

		#endregion //Methods
	}
}

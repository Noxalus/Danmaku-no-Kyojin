using System;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace Danmaku_no_Kyojin.BulletEngine
{
	/// <summary>
	/// This is a complete document that describes a bullet pattern.
	/// </summary>
	public class BulletPattern
	{
		#region Members

		/// <summary>
		/// The root node of a tree structure that describes the bullet pattern
		/// </summary>
		public BulletMLNode RootNode { get; private set; }

		//TODO: move filename class to github and use it here

		/// <summary>
		/// Gets the filename.
		/// This property is only set by calling the parse method
		/// </summary>
		/// <value>The filename.</value>
		public string Filename { get; private set; }

		/// <summary>
		/// the orientation of this bullet pattern: horizontal or veritcal
		/// this is read in from the xml
		/// </summary>
		/// <value>The orientation.</value>
		public EPatternType Orientation { get; private set; }

		#endregion //Members

		#region Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="BulletMLLib.BulletPattern"/> class.
		/// </summary>
		public BulletPattern()
		{
			RootNode = null;
		}

		/// <summary>
		/// convert a string to a pattern type enum
		/// </summary>
		/// <returns>The type to name.</returns>
		/// <param name="str">String.</param>
		private static EPatternType StringToPatternType(string str)
		{
			return (EPatternType)Enum.Parse(typeof(EPatternType), str);
		}
		
		/// <summary>
		/// Parses a bulletml document into this bullet pattern
		/// </summary>
		/// <param name="xmlFileName">Xml file name.</param>
		public bool ParseXML(string xmlFileName)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Ignore;
			
			using (XmlReader reader = XmlReader.Create(xmlFileName, settings))
			{
				//Open the file.
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(reader);
				XmlNode rootXmlNode = xmlDoc.DocumentElement;
				
				//make sure it is actually an xml node
				if (rootXmlNode.NodeType == XmlNodeType.Element)
				{
					//eat up the name of that xml node
					string strElementName = rootXmlNode.Name;
					if (("bulletml" != strElementName) || !rootXmlNode.HasChildNodes)
					{
						//The first node HAS to be bulletml
						Debug.Assert(false);
						return false;
					}

					//Create the root node of the bulletml tree
					RootNode = new BulletMLNode();

					//Read in the whole bulletml tree
					if (!RootNode.Parse(rootXmlNode, null))
					{
						//an error ocurred reading in the tree
						return false;
					}
					Debug.Assert(ENodeName.bulletml == RootNode.Name);

					//Find what kind of pattern this is: horizontal or vertical
					XmlNamedNodeMap mapAttributes = rootXmlNode.Attributes;
					for (int i = 0; i < mapAttributes.Count; i++)
					{
						//will only have the name attribute
						string strName = mapAttributes.Item(i).Name;
						string strValue = mapAttributes.Item(i).Value;
						if ("type" == strName)
						{
							//if  this is a top level node, "type" will be veritcal or horizontal
							Orientation = StringToPatternType(strValue);
						}
					}
				}
				else
				{
					//should be an xml node!!!
					Debug.Assert(false);
					return false;
				}
			}

			//grab that filename 
			Filename = xmlFileName;
			return true;
		}

		#endregion //Methods
	}
}

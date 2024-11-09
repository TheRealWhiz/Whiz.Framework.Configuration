using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Whiz.Framework.Configuration.Xml
{

	///// <summary>
	///// Generic Configuration implementation binded with the xml representation.
	///// Using this class allows facilities in order to save and load configurations from xml files or webservices
	///// </summary>
	//public class XmlConfiguration : GenericConfiguration
	//{

	//	/// <summary>
	//	/// Saves the configuration tree starting from this value to a specific file
	//	/// </summary>
	//	/// <param name="filename"></param>
	//	public void SaveToFile(String filename)
	//	{
	//	}

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	public void Save()
	//	{
	//	}

	//	/// <summary>
	//	/// Loads the xml file specified with filename and creates an in memory representation of it
	//	/// </summary>
	//	/// <param name="filename">Full file name of the xml file (eg: C:\MyDir\config.xml)</param>
	//	public static XmlConfiguration LoadFromFile(String filename)
	//	{
	//		XmlConfiguration ret = new XmlConfiguration();
	//		XDocument xd = XDocument.Load(filename);
	//		ret.Load((XElement)xd.FirstNode, filename);
	//		return ret;
	//	}

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="xmlConfig"></param>
	//	public XmlConfiguration(XElement xmlConfig)
	//	{
	//	}

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="source"></param>
	//	/// <param name="xmlConfig"></param>
	//	private XmlConfiguration(String source, XElement xmlConfig)
	//	{
	//		this.Load(xmlConfig, source);
	//	}

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="value"></param>
	//	/// <param name="attribute"></param>
	//	public XmlConfiguration(String value, Boolean attribute)
	//		: base(value)
	//	{
	//		this.Attribute = attribute;
	//	}

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="source"></param>
	//	/// <param name="value"></param>
	//	/// <param name="attribute"></param>
	//	private XmlConfiguration(String source, String value, Boolean attribute)
	//		: this(value, attribute)
	//	{
	//		this.Source = source;
	//	}

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="value"></param>
	//	public XmlConfiguration(String value)
	//		: base(value)
	//	{
	//	}

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="source"></param>
	//	/// <param name="value"></param>
	//	private XmlConfiguration(String source, String value)
	//		: base(value)
	//	{
	//		this.Source = source;
	//	}

	//	/// <summary>
	//	/// Constructor
	//	/// </summary>
	//	public XmlConfiguration()
	//		: base()
	//	{
	//	}

	//	/// <summary>
	//	/// Load of an included configuration
	//	/// </summary>
	//	/// <param name="includeNode"></param>
	//	/// <returns></returns>
	//	private XmlConfiguration LoadInclude(XElement includeNode)
	//	{
	//		XmlConfiguration ret;
	//		switch (includeNode.Attribute("type").Value)
	//		{
	//			case "file":
	//				ret = LoadFromFile(includeNode.Attribute("source").Value);
	//				break;
	//			default:
	//				ret = null;
	//				break;
	//		}
	//		return ret;
	//	}

	//	/// <summary>
	//	/// Load from a file
	//	/// </summary>
	//	/// <param name="xmlConfig"></param>
	//	/// <param name="source"></param>
	//	private void Load(XElement xmlConfig, String source)
	//	{
	//		_configs = new Dictionary<String, List<GenericConfiguration>>();
	//		foreach (System.Xml.Linq.XElement p in xmlConfig.Elements())
	//		{

	//			// check for the include
	//			if (p.Name == "d3cfgInclude")
	//			{
	//				// include node
	//				XmlConfiguration cfg = LoadInclude(p);
	//				if (_configs.ContainsKey(p.Attribute("name").Value))
	//				{
	//					_configs[p.Attribute("name").Value].Add(cfg);
	//				}
	//				else
	//				{
	//					List<GenericConfiguration> pT = new List<GenericConfiguration>();
	//					pT.Add(cfg);
	//					_configs.Add(p.Attribute("name").Value, pT);
	//				}
	//			}
	//			else
	//			{
	//				// normal configuration
	//				if (p.HasAttributes)
	//				{
	//					foreach (System.Xml.Linq.XAttribute a in p.Attributes())
	//					{
	//						if (_configs.ContainsKey(a.Name.ToString()))
	//						{
	//							_configs[a.Name.ToString()].Add(new XmlConfiguration(source, a.Value, true));
	//						}
	//						else
	//						{
	//							List<GenericConfiguration> pT = new List<GenericConfiguration>();
	//							pT.Add(new XmlConfiguration(source, a.Value, true));
	//							_configs.Add(a.Name.ToString(), pT);
	//						}
	//					}
	//				}

	//				if (p.HasElements)
	//				{
	//					if (_configs.ContainsKey(p.Name.ToString()))
	//					{
	//						XmlConfiguration xc = new XmlConfiguration(source, p);
	//						_configs[p.Name.ToString()].Add(xc);
	//					}
	//					else
	//					{
	//						List<GenericConfiguration> pT = new List<GenericConfiguration>();
	//						XmlConfiguration xc = new XmlConfiguration(source, p);
	//						pT.Add(xc);
	//						_configs.Add(p.Name.ToString(), pT);
	//					}
	//				}
	//				else
	//				{
	//					if (_configs.ContainsKey(p.Name.ToString()))
	//					{
	//						_configs[p.Name.ToString()].Add(new XmlConfiguration(source, p.Value));
	//					}
	//					else
	//					{
	//						List<GenericConfiguration> pT = new List<GenericConfiguration>();
	//						pT.Add(new XmlConfiguration(source, p.Value));
	//						_configs.Add(p.Name.ToString(), pT);
	//					}
	//				}
	//			}
	//		}
	//	}

	//	/// <summary>
	//	/// Gets a config at a specified path. If the specified path doesn't exists it will return a XmlConfiguration with Value = null.
	//	/// </summary>
	//	/// <param name="path">The path in the form "level/sublevel[index]/item"</param>
	//	/// <returns>The config</returns>
	//	public new XmlConfiguration Get(String path)
	//	{
	//		GenericConfiguration gc = base.Get(path);
	//		if (gc.GetType() == typeof(XmlConfiguration))
	//		{
	//			return (XmlConfiguration)gc;
	//		}
	//		else
	//		{
	//			return new XmlConfiguration(value: null);
	//		}
	//	}

	//	/// <summary>
	//	/// Gets a config at a certain path. If the item doesn't exists it will throw a ConfigurationException if mandatory is true, elsewhere it will return a new XmlConfiguration with Value = null.
	//	/// </summary>
	//	/// <param name="path">The path in the form "level/sublevel[index]/item"</param>
	//	/// <param name="mandatory">Boolean which causes the throw of a ConfigurationException if the specified path doesn't exist.</param>
	//	/// <returns>The config</returns>
	//	public new XmlConfiguration Get(String path, Boolean mandatory)
	//	{
	//		return (XmlConfiguration)base.Get(path, mandatory);
	//	}

	//	/// <summary>
	//	/// Gets a config at a specified path. If the specified path doesn't exists it will return a XmlConfiguration with the defaultValue specified.
	//	/// </summary>
	//	/// <param name="path">The path in the form "level/sublevel[index]/item"</param>
	//	/// <param name="defaultValue">The value to be returned if the specified path doesn't exists.</param>
	//	/// <returns>The config or null</returns>
	//	public new XmlConfiguration Get(String path, String defaultValue)
	//	{
	//		GenericConfiguration gc = base.Get(path);
	//		if (gc.GetType() == typeof(XmlConfiguration))
	//		{
	//			return (XmlConfiguration)gc;
	//		}
	//		else
	//		{
	//			return new XmlConfiguration(value: null);
	//		}
	//	}

	//	/// <summary>
	//	/// The source in which the Xml Configuration were originally (empty if runtime added)
	//	/// </summary>
	//	private String Source { get; set; }

	//	/// <summary>
	//	/// Indication of alteration of the current value
	//	/// </summary>
	//	private Boolean Modified { get; set; }

	//	/// <summary>
	//	/// Indicates if the value was stored in an attribute
	//	/// </summary>
	//	private Boolean Attribute { get; set; }

	//}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Whiz.Framework.Configuration.Xml
{
	/// <summary>
	/// Extensions to integrate xml representation of the configuration
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Loads the MemoryConfiguration starting from 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="xmlConfig"></param>
		/// <param name="readAttributes"></param>
		public static void LoadFromXml(this GenericConfiguration config, XElement xmlConfig, Boolean readAttributes = false)
		{
			config._configs = new Dictionary<String, List<GenericConfiguration>>();
			// adding the root element
			GenericConfiguration gc = new GenericConfiguration();
			List<GenericConfiguration> t = new List<GenericConfiguration>();
			t.Add(gc);
			if (xmlConfig.HasAttributes && readAttributes)
			{
				LoadAttributes(gc, xmlConfig);
			}
			config._configs.Add(xmlConfig.Name.ToString(), t);
			LoadXml(gc, xmlConfig, readAttributes);

		}

		private static void LoadXml(GenericConfiguration config, XElement xmlConfig, Boolean readAttributes)
		{
			foreach (System.Xml.Linq.XElement p in xmlConfig.Elements())
			{
				// check for the include
				if (p.Name == "wcfgInclude")
				{
					// include node
					GenericConfiguration cfg = LoadInclude(p, readAttributes);
					if (config._configs.ContainsKey(p.Attribute("name").Value))
					{
						config._configs[p.Attribute("name").Value].Add(cfg);
					}
					else
					{
						List<GenericConfiguration> pT = new List<GenericConfiguration>();
						pT.Add(cfg);
						config._configs.Add(p.Attribute("name").Value, pT);
					}
				}
				else
				{
					if (p.HasElements)
					{
						if (config._configs.ContainsKey(p.Name.ToString()))
						{
							GenericConfiguration gc = new GenericConfiguration();
							LoadXml(gc, p, readAttributes);
							if (readAttributes && p.HasAttributes) LoadAttributes(gc, p);
							config._configs[p.Name.ToString()].Add(gc);
						}
						else
						{
							List<GenericConfiguration> pT = new List<GenericConfiguration>();
							GenericConfiguration gc = new GenericConfiguration();
							LoadXml(gc, p, readAttributes);
							if (readAttributes && p.HasAttributes) LoadAttributes(gc, p);
							pT.Add(gc);
							config._configs.Add(p.Name.ToString(), pT);
						}
					}
					else
					{
						if (config._configs.ContainsKey(p.Name.ToString()))
						{
							GenericConfiguration gc = new GenericConfiguration(p.Value);
							if (readAttributes && p.HasAttributes) LoadAttributes(gc, p);
							config._configs[p.Name.ToString()].Add(gc);
						}
						else
						{
							List<GenericConfiguration> pT = new List<GenericConfiguration>();
							GenericConfiguration gc = new GenericConfiguration(p.Value);
							if (readAttributes && p.HasAttributes) LoadAttributes(gc, p);
							pT.Add(gc);
							config._configs.Add(p.Name.ToString(), pT);
						}
					}
				}
			}
		}

		private static void LoadAttributes(GenericConfiguration config, XElement p)
		{
			foreach (System.Xml.Linq.XAttribute a in p.Attributes())
			{
				if (config._configs.ContainsKey(a.Name.ToString()))
				{
					config._configs[a.Name.ToString()].Add(new GenericConfiguration(a.Value));
				}
				else
				{
					List<GenericConfiguration> pT = new List<GenericConfiguration>();
					pT.Add(new GenericConfiguration(a.Value));
					config._configs.Add(a.Name.ToString(), pT);
				}
			}
		}

		/// <summary>
		/// Load of an included configuration
		/// </summary>
		/// <param name="includeNode"></param>
		/// <param name="readAttributes"></param>
		/// <returns></returns>
		private static GenericConfiguration LoadInclude(XElement includeNode, Boolean readAttributes)
		{
			GenericConfiguration ret;
			switch (includeNode.Attribute("type").Value)
			{
				case "file":
					ret = LoadFromXmlFile(includeNode.Attribute("source").Value, readAttributes);
					break;
				default:
					ret = null;
					break;
			}
			return ret;
		}

		/// <summary>
		/// Loads configuration from the target Xml File
		/// </summary>
		/// <param name="config"></param>
		/// <param name="filename"></param>
		/// <param name="readAttributes"></param>
		public static void LoadFromXmlFile(this GenericConfiguration config, String filename, Boolean readAttributes)
		{
			XDocument xd = XDocument.Load(filename);
			config.LoadFromXml((XElement)xd.Root, readAttributes);
		}

		private static GenericConfiguration LoadFromXmlFile(String filename, Boolean readAttributes)
		{
			GenericConfiguration ret = new GenericConfiguration();
			XDocument xd = XDocument.Load(filename);
			ret.LoadFromXml((XElement)xd.Root, readAttributes);
			return ret;
		}

	}
}

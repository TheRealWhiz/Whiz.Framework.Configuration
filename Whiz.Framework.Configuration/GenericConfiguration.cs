using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Whiz.Framework.Configuration;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Whiz.Framework.Configuration.Xml")]

namespace Whiz.Framework.Configuration
{
	/// <summary>
	/// Class representing the results of a Search into GenericConfiguration. It is basilarly a Dictionary with
	/// Key representing the exploded path where the configuration is found and Value is a ReadOnlyCollection
	/// with the configurations found
	/// </summary>
	public class GenericConfigurationSearchResult : Dictionary<String, ReadOnlyCollection<GenericConfiguration>>
	{
	}

	/// <summary>
	/// A generic configuration in memory.
	/// </summary>
	[Serializable]
	public class GenericConfiguration
	{
		/// <summary>
		/// The value of the current key
		/// </summary>
		public String Value { get; set; }

		/// <summary>
		/// The configuration keys at this level
		/// </summary>
		internal Dictionary<String, List<GenericConfiguration>> _configs;

		/// <summary>
		/// Gets a config at a specified path. If the specified path doesn't exists it will return a GenericConfiguration with Value = null.
		/// </summary>
		/// <param name="path">The path in the form "level/sublevel[index]/item"</param>
		/// <returns>The GenericConfiguration</returns>
		public GenericConfiguration Get(String path)
		{
			return Get(path, null);
		}

		/// <summary>
		/// Gets a config at a specified path. If the specified path doesn't exists it will return a GenericConfiguration with the defaultValue specified.
		/// </summary>
		/// <param name="path">The path in the form "level/sublevel[index]/item"</param>
		/// <param name="defaultValue">The value to be returned if the specified path doesn't exists.</param>
		/// <returns>The GenericConfiguration or null</returns>
		public GenericConfiguration Get(String path, String defaultValue)
		{
			GenericConfiguration current = this;
			foreach (String p in path.Split('/'))
			{
				if (p.Contains('['))
				{
					String[] pp = p.Split('[');
					current = current[pp[0], Int32.Parse(pp[1].Replace("]", ""))];
				}
				else
				{
					current = current[p, 0];
				}
				if (current == null)
				{
					return new GenericConfiguration(defaultValue);
				}
			}
			return current;
		}

		/// <summary>
		/// Sets a config at a specified path. If the specified path doesn't exists it will create it.
		/// If you specify a path with index ("root/level[3]") and there isn't a level[3] it will throw an exception
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="value">The value to set/create</param>
		public void Set(String path, GenericConfiguration value)
		{
			Set(path, value, true, false);
		}

		/// <summary>
		/// Sets a config at a specified path. If the specified path doesn't exists it will create it if create is true.
		/// If you specify a path with index ("root/level[3]") and there isn't a level[3] it will throw an exception
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="value">The value to set/create</param>
		/// <param name="create">If create is false it will not create the configs for the value (only updates existing configs)</param>
		public void Set(String path, GenericConfiguration value, Boolean create)
		{
			Set(path, value, create, false);
		}

		/// <summary>
		/// Sets a config value at a specified path. If the specified path doesn't exists it will create it.
		/// If you specify a path with index ("root/level[3]") and there isn't a level[3] it will throw an exception
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="value">The new value to set/create</param>
		public void Set(String path, String value)
		{
			Set(path, new GenericConfiguration() { Value = value }, true, false);
		}

		/// <summary>
		/// Sets a config value at a specified path. If the specified path doesn't exists it will create it if create is true.
		/// If you specify a path with index ("root/level[3]") and there isn't a level[3] it will throw an exception
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="value">The value to set/create</param>
		/// <param name="create">If create is false it will not create the configs for the value (only updates existing configs)</param>
		public void Set(String path, String value, Boolean create)
		{
			Set(path, new GenericConfiguration() { Value = value }, create, false);
		}

		/// <summary>
		/// Adds a config at a specified path. If the specified path doesn't exists it will create it.
		/// If you specify a path with index ("root/level[3]") and there isn't a level[3] it will throw an exception
		/// If the config already exists it will add a new config with the same key
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="value">The new value to add</param>
		public void Add(String path, GenericConfiguration value)
		{
			Set(path, value, true, true);
		}

		/// <summary>
		/// Adds a config value at a specified path. If the specified path doesn't exists it will create it.
		/// If you specify a path with index ("root/level[3]") and there isn't a level[3] it will throw an exception
		/// If the config already exists it will add a new config with the same key
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="value">The new value to add</param>
		public void Add(String path, String value)
		{
			Set(path, new GenericConfiguration() { Value = value }, true, true);
		}

		/// <summary>
		/// Adds or sets config values eventually creating path. Add and Set methods will call this one internally
		/// </summary>
		/// <param name="path">The path</param>
		/// <param name="value">The value</param>
		/// <param name="create">Indicator for creating the structure</param>
		/// <param name="add">Indicator for always add or update</param>
		internal void Set(String path, GenericConfiguration value, Boolean create, Boolean add)
		{
			GenericConfiguration current = this;
			GenericConfiguration parent = this;
			Int32 i = 0;
			Int32 pathCount = path.Split('/').Length;
			foreach (String p in path.Split('/'))
			{
				if (i == pathCount - 1)
				{
					if (add)
					{
						parent.AddKey(p, value);
					}
					else if (parent._configs.ContainsKey(p))
					{
						parent._configs[p][0] = value;
					}
					else if (create)
					{
						parent.AddKey(p, value);
					}
					else
					{
						throw new ConfigurationException("Value not found");
					}
				}
				else
				{
					if (p.Contains('['))
					{
						String[] pp = p.Split('[');
						current = current[pp[0], Int32.Parse(pp[1].Replace("]", ""))];
					}
					else
					{
						current = current[p, 0];
					}
					if (current == null)
					{
						if (p.Contains('['))
						{
							throw new ConfigurationException("Value not found");
						}
						else
						{
							if (create)
							{
								parent.AddLevel(p);
								parent = parent[p, 0];
								current = parent;
							}
							else
							{
								throw new ConfigurationException("Value not found");
							}
						}
					}
					else
					{
						parent = current;
					}
				}
				i++;
			}
		}

		/// <summary>
		/// Gets a config at a certain path. If the item doesn't exists it will throw a ConfigurationException if mandatory is true, elsewhere it will return a new GenericConfiguration with Value = null.
		/// </summary>
		/// <param name="path">The path in the form "level/sublevel[index]/item"</param>
		/// <param name="mandatory">Boolean which causes the throw of a ConfigurationException if the specified path doesn't exist.</param>
		/// <returns>The config</returns>
		public GenericConfiguration Get(String path, Boolean mandatory)
		{
			GenericConfiguration p = Get(path, null);
			if (p.Value == null)
			{
				throw new ConfigurationException("Missing mandatory configuration for {0}", path);
			}
			else
			{
				return p;
			}
		}

		/// <summary>
		/// Gets all the configs of the given certain path.
		/// </summary>
		/// <param name="path">The path in the form "level/sublevel[index]/item"</param>
		/// <returns>The configs found or an empty readonly List if none found</returns>
		public ReadOnlyCollection<GenericConfiguration> GetList(String path)
		{
			GenericConfiguration current = this;
			Int32 i = 0;
			Int32 pathCount = path.Split('/').Length;
			foreach (String p in path.Split('/'))
			{
				if (i == pathCount - 1)
				{
					if (current[p] != null)
					{
						return current[p];
					}
					else
					{
						return new List<GenericConfiguration>().AsReadOnly();
					}
				}
				else
				{
					if (p.Contains('['))
					{
						String[] pp = p.Split('[');
						current = current[pp[0], Int32.Parse(pp[1].Replace("]", ""))];
					}
					else
					{
						current = current[p, 0];
					}
					if (current == null)
					{
						return new List<GenericConfiguration>().AsReadOnly();
					}
				}
				i++;
			}
			return new List<GenericConfiguration>().AsReadOnly();
		}

		/// <summary>
		/// Initialize a new instance with the specified Value
		/// </summary>
		/// <param name="value">The Value</param>
		public GenericConfiguration(String value)
		{
			_configs = new Dictionary<String, List<GenericConfiguration>>();
			Value = value;
		}

		/// <summary>
		/// Initialize a new instance
		/// </summary>
		public GenericConfiguration()
		{
			_configs = new Dictionary<String, List<GenericConfiguration>>();
		}

		/// <summary>
		/// Navigation for the sublevels
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public ReadOnlyCollection<GenericConfiguration> this[String key]
		{
			get
			{
				if (_configs.ContainsKey(key))
				{
					return _configs[key].AsReadOnly();
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Navigation for the sublevels
		/// </summary>
		/// <param name="key"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public GenericConfiguration this[String key, Int32 index]
		{
			get { if (_configs.ContainsKey(key)) { return _configs[key][index]; } else { return null; } }
		}

		/// <summary>
		/// The whole block of sub configuration of this level
		/// </summary>
		public Dictionary<String, List<GenericConfiguration>> Configs
		{
			get
			{
				return _configs;
			}
			set
			{
				_configs = value;
			}
		}

		/// <summary>
		/// Adds the specified value to the given key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AddKey(String key, GenericConfiguration value)
		{
			if (_configs.ContainsKey(key))
			{
				_configs[key].Add(value);
			}
			else
			{
				List<GenericConfiguration> pT = new List<GenericConfiguration>();
				pT.Add(value);
				_configs.Add(key, pT);
			}
		}

		/// <summary>
		/// Adds the specified values to the given key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="values"></param>
		public void AddKey(String key, List<GenericConfiguration> values)
		{
			if (_configs.ContainsKey(key))
			{
				_configs[key].AddRange(values);
			}
			else
			{
				_configs.Add(key, values);
			}
		}

		/// <summary>
		/// Adds a sub key
		/// </summary>
		/// <param name="key"></param>
		public void AddLevel(String key)
		{
			AddKey(key, new GenericConfiguration());
		}

		/// <summary>
		/// Removes a key from the sub configurations
		/// </summary>
		/// <param name="key"></param>
		public void RemoveKey(String key)
		{
			if (_configs.ContainsKey(key))
			{
				_configs.Remove(key);
			}
		}

		/// <summary>
		/// Clears the configs from this element. It will set Value to null and removes all sub configurations
		/// </summary>
		public void Clear()
		{
			_configs.Clear();
			Value = null;
		}

		/// <summary>
		/// Search for configurations following a path specified in searchString
		/// </summary>
		/// <param name="searchString">Path to follow in the form a/b/c. It will accept the wildcard * in order to search in all the nodes at the level. es: a/*/c.</param>
		/// <returns>A GenericConfigurationSearchResult containing the result of the search</returns>
		public GenericConfigurationSearchResult Search(String searchString)
		{
			return InternalSearch(searchString, "");
		}

		/// <summary>
		/// Internal method for recursive search
		/// </summary>
		/// <param name="searchString"></param>
		/// <param name="path"></param>
		/// <returns>The result of the search</returns>
		internal GenericConfigurationSearchResult InternalSearch(String searchString, String path)
		{
			GenericConfigurationSearchResult res = new GenericConfigurationSearchResult();
			String[] pathParts = searchString.Split('/');
			// we're at the last level
			if (pathParts.Length == 1)
			{
				if (pathParts[0] == "*")
				{
					foreach (KeyValuePair<String, List<GenericConfiguration>> kvp in _configs)
					{
						res.Add(path + "/" + kvp.Key, kvp.Value.AsReadOnly());
					}
				}
				else if (_configs.Keys.Contains(pathParts[0]))
				{
					res.Add(path + "/" + pathParts[0], this[pathParts[0]]);
				}
			}
			else
			{
				if (pathParts[0] == "*")
				{
					foreach (KeyValuePair<String, List<GenericConfiguration>> kvp in _configs)
					{
						foreach (GenericConfiguration gc in kvp.Value)
						{
							foreach (KeyValuePair<String, ReadOnlyCollection<GenericConfiguration>> found in gc.InternalSearch(searchString.Substring(2, searchString.Length - 2), path + "/" + kvp.Key))
							{
								res.Add(found.Key, found.Value);
							}
						}
					}
				}
				else if (_configs.Keys.Contains(pathParts[0]))
				{
					foreach (GenericConfiguration gc in _configs[pathParts[0]])
					{
						foreach (KeyValuePair<String, ReadOnlyCollection<GenericConfiguration>> found in gc.InternalSearch(searchString.Substring(pathParts[0].Length + 1, searchString.Length - 2), path + "/" + pathParts[0]))
						{
							res.Add(found.Key, found.Value);
						}
					}
				}
			}
			return res;
		}

	}

}

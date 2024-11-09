using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Whiz.Framework.Configuration
{
	/// <summary>
	/// Configuration Exception class
	/// </summary>
	public class ConfigurationException : Exception
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="pars"></param>
		public ConfigurationException(string message, params string[] pars) : base(string.Format(message, pars)) { }
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		/// <param name="pars"></param>
		public ConfigurationException(string message, Exception innerException, params string[] pars) : base(string.Format(message, pars), innerException) { }
	}
}

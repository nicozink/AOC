using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
	/// A solution for a specific day.
	/// </summary>
	public class SolutionClassAttribute : Attribute
	{
		public static SolutionClassAttribute GetAttribute(Type t)
		{
			var attrs = System.Attribute.GetCustomAttributes(t);

			return (Common.SolutionClassAttribute)attrs.FirstOrDefault(x => x is Common.SolutionClassAttribute);
		}

		/// <summary>
		/// The specific day.
		/// </summary>
		public int Day
        {
			get;
			set;
        }
	}
}

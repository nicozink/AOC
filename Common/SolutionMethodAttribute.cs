using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
    /// <summary>
	/// A solution for a specific day.
	/// </summary>
	public class SolutionMethodAttribute : Attribute
	{
		public static SolutionMethodAttribute GetAttribute(MethodInfo mi)
		{
			return (SolutionMethodAttribute)mi.GetCustomAttributes(typeof(Common.SolutionMethodAttribute), false).FirstOrDefault();
		}

		/// <summary>
		/// The part of the solution.
		/// </summary>
		public int Part
        {
			get;
			set;
        }
	}
}

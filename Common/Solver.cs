using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
	/// <summary>
	/// A solution for a specific day.
	/// </summary>
	public class Solver
	{
		static Solver()
        {
			// Get all classes that have the SolutionClass attribute.
			// These hold the solutions for each day.
			types = Assembly.Load("Solutions").GetTypes()
					.Where(x => SolutionClassAttribute.GetAttribute(x) != null)
					.ToDictionary(x => SolutionClassAttribute.GetAttribute(x)!.Day);
		}

		/// <summary>
		/// Checks whether a solution for a specific day
		/// exists.
		/// </summary>
		/// <param name="day">The day.</param>
		/// <returns>True if the solution exists.</returns>
		public static bool HasSolution(int day)
        {
			return types.ContainsKey(day);
		}

		/// <summary>
		/// Run a specific solution for a given day.
		/// </summary>
		/// <param name="day">The day.</param>
		/// <param name="part">The solution part.</param>
		public static void Run(int day, int part)
		{
			var solutionType = types[day];

			var classAttribute = SolutionClassAttribute.GetAttribute(solutionType);

			var instance = Activator.CreateInstance(solutionType);

			var methods = solutionType.GetMethods()
				.Where(m => SolutionMethodAttribute.GetAttribute(m) != null)
				.ToDictionary(x => SolutionMethodAttribute.GetAttribute(x)!.Part);

			if (methods.ContainsKey(part))
			{
				var obj = methods[part].Invoke(instance, null);
				Console.WriteLine(obj);
			}
		}

		/// <summary>
		/// Stores the system types that were registered
		/// as solutions.
		/// </summary>
		private static readonly Dictionary<int, Type> types;
	}
}

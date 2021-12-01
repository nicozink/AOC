using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class Program
{
	/// <summary>
	/// Runs the solutions for each day. Takes parameters to run
	/// a specific solution or part solution - e.g. 1, 3-1, 5-2.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	static void Main(string[] args)
	{
		Console.WriteLine("Welcome to Advent of Code 2021!");
		Console.WriteLine();

		if (args.Length == 1)
		{
			// Take the parameters, and split them into parts.
			// E.g. 3-1 means run part 1 of day 3.

			var parts = args[0]
				.Split('-')
				.Select(x => int.Parse(x))
				.ToList();

			int selectedDay = parts[0];
			int selectedIndex = selectedDay - 1;

			int? selectedPart = null;
			if (parts.Count() == 2)
			{
				selectedPart = parts[1];
			}

			InvokeSolution(selectedDay, selectedPart);
		}
		else
		{
			// If we don't have any parameters, run all solutions.

			for (int i = 1; i <= 25; ++i)
			{
				InvokeSolution(i, null);

				Console.WriteLine();
			}
		}
    }

	/// <summary>
	/// Invokes the solution for a specific day.
	/// </summary>
	/// <param name="type">The class type for the soltion.</param>
	/// <param name="part">The part of the solution to run, or null to run all</param>
	static void InvokeSolution(int day, int? part)
    {
		if (Solver.HasSolution(day))
		{
			Console.WriteLine("Day {0}", day);

			if (!part.HasValue || part == 1)
			{
				Solver.Run(day, 1);
			}

			if (!part.HasValue || part == 2)
			{
				Solver.Run(day, 2);
			}
		}
	}
}

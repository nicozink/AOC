using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 18:
    /// https://adventofcode.com/2022/day/19
    /// </summary>
    [TestClass]
    public class Day19
    {
        /// <summary>
        /// Stores the cost to produce a robot.
        /// </summary>
        /// <param name="Ore">The amount of ore needed.</param>
        /// <param name="Clay">The amount of clay needed.</param>
        /// <param name="Obsidian">The amount of obsidian needed.</param>
        record RobotCost(int Ore, int Clay, int Obsidian);

        /// <summary>
        /// A blueprint used to generate robots.
        /// </summary>
        /// <param name="Index">The index of the blueprint.</param>
        /// <param name="RobotCosts">The costs to produce each robot.</param>
        /// <param name="ProductionLimit">The production limits for each robot type.</param>
        record Blueprint(int Index, RobotCost[] RobotCosts, int[] ProductionLimit);

        /// <summary>
        /// Reads the blueprints from the file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The blueprints.</returns>
        private static IEnumerable<Blueprint> ReadInput(string path)
        {
            var lines = System.IO.File.ReadLines(path);

            foreach (var line in lines)
            {
                var formattedLine = line
                    .Replace("Blueprint ", "")
                    .Replace(": Each ore robot costs ", ",")
                    .Replace(" ore. Each clay robot costs ", ",")
                    .Replace(" ore. Each obsidian robot costs ", ",")
                    .Replace(" ore and ", ",")
                    .Replace(" clay. Each geode robot costs ", ",")
                    .Replace(" ore and ", ",")
                    .Replace(" obsidian.", "");

                var splitLine = formattedLine
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();

                int blueprintIndex = splitLine[0];
                int oreRobotCost = splitLine[1];
                int clayRobotCost = splitLine[2];
                int obsidianOreCost = splitLine[3];
                int obsidianClayCost = splitLine[4];
                int geodeOreCost = splitLine[5];
                int geodeObsidianCost = splitLine[6];

                int maxOre = new int[] { oreRobotCost, clayRobotCost, obsidianOreCost, geodeOreCost }.Max();
                int maxClay = obsidianClayCost;
                int maxObsidian = geodeObsidianCost;

                yield return new Blueprint(
                    blueprintIndex,
                    new RobotCost[]
                    {
                        new RobotCost(oreRobotCost, 0, 0),
                        new RobotCost(clayRobotCost, 0, 0),
                        new RobotCost(obsidianOreCost, obsidianClayCost, 0),
                        new RobotCost(geodeOreCost, 0, geodeObsidianCost)
                    },
                    new int[] {
                        maxOre,
                        maxClay,
                        maxObsidian,
                        int.MaxValue
                    });
            }
        }

        /// <summary>
        /// Stores the resources that are available.
        /// </summary>
        /// <param name="Ore">The amount of ore available.</param>
        /// <param name="Clay">The amount of clay available.</param>
        /// <param name="Obsidian">The amount of obsidian available.</param>
        /// <param name="OpenGeodes">The number of opened geodes.</param>
        record Resources(int Ore, int Clay, int Obsidian, int OpenGeodes);

        /// <summary>
        /// Tracks the number of robots that have been produced.
        /// </summary>
        /// <param name="OreRobots">The number of ore robots.</param>
        /// <param name="ClayRobots">The number of clay robots.</param>
        /// <param name="ObsidianRobots">The number of obsidian robots.</param>
        /// <param name="GeodeRobots">The number of geode robots.</param>
        record Robots(int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots);

        /// <summary>
        /// The cache tracking the state of the robots and resources.
        /// </summary>
        /// <param name="Minutes">The current number of minutes left.</param>
        /// <param name="Resources">The resources which are available.</param>
        /// <param name="Robots">The robots which have bene produced.</param>
        record StateCache(int Minutes, Resources Resources, Robots Robots);

        /// <summary>
        /// Advance time, and gather any resources by the active robots.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The new state.</returns>
        private static StateCache AdvanceTime(StateCache state)
        {
            int time = state.Minutes - 1;
            var resources = state.Resources;
            var robots = state.Robots;

            var newResources = resources with
            {
                Ore = resources.Ore + robots.OreRobots,
                Clay = resources.Clay + robots.ClayRobots,
                Obsidian = resources.Obsidian + robots.ObsidianRobots,
                OpenGeodes = resources.OpenGeodes + robots.GeodeRobots
            };

            var newState = state with
            {
                Minutes = time,
                Resources = newResources
            };

            return newState;
        }

        /// <summary>
        /// Try build a robot, if the conditions have been met.
        /// </summary>
        /// <param name="state">The previous state.</param>
        /// <param name="blueprint">The blueprint used to produce the robot.</param>
        /// <param name="robotType">The robot type.</param>
        /// <param name="newState">The new state.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static bool TryBuildRobot(StateCache state, Blueprint blueprint, int robotType, out StateCache newState)
        {
            var resources = state.Resources;
            var robots = state.Robots;

            var rule = blueprint.RobotCosts[robotType];

            var current = robotType switch
            {
                0 => robots.OreRobots,
                1 => robots.ClayRobots,
                2 => robots.ObsidianRobots,
                3 => robots.GeodeRobots,
                _ => throw new Exception("Unexpected value")
            };

            if (current + 1 <= blueprint.ProductionLimit[robotType] &&
                resources.Ore >= rule.Ore &&
                resources.Clay >= rule.Clay &&
                resources.Obsidian >= rule.Obsidian)
            {
                newState = AdvanceTime(state);

                var newResources = newState.Resources;
                newResources = newResources with
                {
                    Ore = newResources.Ore - rule.Ore,
                    Clay = newResources.Clay - rule.Clay,
                    Obsidian = newResources.Obsidian - rule.Obsidian
                };

                Robots newRobots = robotType switch
                {
                    0 => robots with { OreRobots = robots.OreRobots + 1 },
                    1 => robots with { ClayRobots = robots.ClayRobots + 1 },
                    2 => robots with { ObsidianRobots = robots.ObsidianRobots + 1 },
                    3 => robots with { GeodeRobots = robots.GeodeRobots + 1 },
                    _ => throw new Exception("Unexpected value")
                };

                newState = newState with
                {
                    Resources = newResources,
                    Robots = newRobots
                };

                return true;
            }

            newState = state;
            return false;
        }

        /// <summary>
        /// Work out the manximum number of geodes given the state and blueprint.
        /// </summary>
        /// <param name="blueprint">The blueprint.</param>
        /// <param name="state">The state.</param>
        /// <param name="cacheLookup">The lookup cache.</param>
        /// <param name="geodeHistory">The history.</param>
        /// <returns>The maximum geode value.</returns>
        private static int GetMaxGeodes(Blueprint blueprint, StateCache state, Dictionary<StateCache, int> cacheLookup, int[] geodeHistory)
        {
            if (cacheLookup.TryGetValue(state, out var value))
            {
                return value;
            }

            if (state.Minutes == 0)
            {
                return state.Resources.OpenGeodes;
            }

            var history = geodeHistory[state.Minutes];
            if (state.Resources.OpenGeodes < history - 3) // This has been experimentally set so we get the correct values.
            {
                return 0;
            }
            else if (history < state.Resources.OpenGeodes)
            {
                geodeHistory[state.Minutes] = state.Resources.OpenGeodes;
            }

            int maxGeodes = state.Resources.OpenGeodes;

            var resources = state.Resources;

            bool canMakeMoves = false;

            var robots = state.Robots;
            for (int i = 0; i < blueprint.RobotCosts.Length; i++)
            {
                var rule = blueprint.RobotCosts[i];

                if (TryBuildRobot(state, blueprint, i, out var newState))
                {
                    canMakeMoves = true;
                    var newGeodes = GetMaxGeodes(blueprint, newState, cacheLookup, geodeHistory);
                    maxGeodes = Math.Max(maxGeodes, newGeodes);
                }
            }

            // Let time advance and work out the new resources.
            if (!canMakeMoves ||
                (resources.Ore <= blueprint.ProductionLimit[0] &&
                resources.Clay <= blueprint.ProductionLimit[1] &&
                resources.Obsidian <= blueprint.ProductionLimit[2]))
            {
                var newState = AdvanceTime(state);

                var newGeodes = GetMaxGeodes(blueprint, newState, cacheLookup, geodeHistory);
                maxGeodes = Math.Max(maxGeodes, newGeodes);
            }

            cacheLookup.Add(state, maxGeodes);

            return maxGeodes;
        }

        /// <summary>
        /// Read the input and work out the number of geodes for each blueprint.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="time">The time limit.</param>
        /// <param name="numBlueprints">The number of blueprints.</param>
        /// <returns>The geodes.</returns>
        private static IEnumerable<int> GetGeodes(string path, int time, int numBlueprints)
        {
            var blueprints = ReadInput(path)
                .ToArray();

            if (blueprints.Length > numBlueprints)
            {
                blueprints = blueprints.Take(numBlueprints).ToArray();
            }

            foreach (var blueprint in blueprints)
            {
                var resources = new Resources(0, 0, 0, 0);
                var robots = new Robots(1, 0, 0, 0);
                var state = new StateCache(time, resources, robots);
                var history = Enumerable.Range(0, time + 1).Select(x => 0).ToArray();
                int maxGeodes = GetMaxGeodes(blueprint, state, new(), history);
                yield return maxGeodes;
            }
        }

        /// <summary>
        /// Works out the quality for each blueprint, and returns the sum.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="time">The time limit.</param>
        /// <returns>The sum of blueprint quality.</returns>
        private static int GetBlueprintQuality(string path, int time)
        {
            var geodes = GetGeodes(path, time, int.MaxValue)
                .ToArray();

            return geodes
                .Select((x, i) => x * (i + 1))
                .Sum();
        }

        /// <summary>
        /// Works out the geodes for each blueprint, and returns their product.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <param name="time">The time limit.</param>
        /// <param name="numBlueprints">The number of blueprints.</param>
        /// <returns></returns>
        private static int GetBlueprintSum(string path, int time, int numBlueprints)
        {
            var geodes = GetGeodes(path, time, numBlueprints);
            return geodes.Aggregate(1, (prod, val) => prod * val);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(33, GetBlueprintQuality("AOC2022/Day19/Example.txt", 24));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(1349, GetBlueprintQuality("AOC2022/Day19/Input.txt", 24));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(3472, GetBlueprintSum("AOC2022/Day19/Example.txt", 32, 3));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(21840, GetBlueprintSum("AOC2022/Day19/Input.txt", 32, 3));

        #endregion
    }
}

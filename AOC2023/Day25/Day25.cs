using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 25:
    /// https://adventofcode.com/2023/day/25
    /// </summary>
    [TestClass]
    public class Day25
    {
        /// <summary>
        /// Reads the graph as connections between nodes from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The graph.</returns>
        private static Dictionary<string, List<string>> ReadInput(string path)
        {
            var output = new Dictionary<string, List<string>>();

            var lines = System.IO.File.ReadLines(path);
            foreach (var line in lines)
            {
                var connections = line
                    .Replace(":", "")
                    .Split()
                    .ToArray();

                var source = connections[0];
                for (int i = 1; i < connections.Length; i++)
                {
                    var destination = connections[i];

                    if (!output.TryGetValue(source, out var sourceList))
                    {
                        output[source] = sourceList = new();
                    }

                    if (!output.TryGetValue(destination, out var destinationList))
                    {
                        output[destination] = destinationList = new();
                    }

                    sourceList.Add(destination);
                    destinationList.Add(source);
                }
            }

            return output;
        }

        /// <summary>
        /// Stores an edge in the graph which connects two nodes.
        /// </summary>
        /// <param name="From">The from node.</param>
        /// <param name="To">The to node.</param>
        record Edge(string From, string To);

        /// <summary>
        /// Gets the discinct edges from the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns>The distinct edges.</returns>
        private static Edge[] GetDistinctEdges(Dictionary<string, List<string>> graph)
        {
            var distinctPaths = new HashSet<Edge>();

            var allPaths = graph
                .SelectMany(x => x.Value.Select(y => new Edge(x.Key, y)));
            foreach (var path in allPaths)
            {
                var opposite = new Edge(path.To, path.From);
                if (!distinctPaths.Contains(opposite))
                {
                    distinctPaths.Add(path);
                }
            }

            return distinctPaths.ToArray();
        }

        /// <summary>
        /// Stores a weight that is assigned to each edge in the graph.
        /// </summary>
        /// <param name="Edge">The edge.</param>
        /// <param name="Weight">The weight of the edge.</param>
        private record EdgeWeight(Edge Edge, int Weight);

        /// <summary>
        /// Calculates a weight for each edge, based on how common it is in
        /// a traversal of all nodes in the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="edges">The distinct edges in the graph.</param>
        /// <returns>A weight for each distinct edge in the graph.</returns>
        private static IEnumerable<EdgeWeight> GetEdgeWeights(Dictionary<string, List<string>> graph,
            Edge[] edges)
        {
            var edgeWeights = new int[edges.Length];
            foreach (var node in graph.Keys)
            {
                var distances = new Dictionary<string, int>
                {
                    [node] = 0
                };

                var frontier = new Queue<string>();
                frontier.Enqueue(node);

                while (frontier.Count > 0)
                {
                    var startNode = frontier.Dequeue();
                    var distance = distances[startNode];

                    foreach (var nextNode in graph[startNode])
                    {
                        if (!distances.TryGetValue(nextNode, out var existingDistance) ||
                            existingDistance > distance)
                        {
                            var newDistance = distance + 1;
                            distances[nextNode] = newDistance;

                            frontier.Enqueue(nextNode);
                        }
                    }
                }

                // Extract the weight for each edge. The weigth is based on the distance,
                // which we invert to make it fall off the farther we go. The idea is that
                // we end up with a weighted list of edges, where common ones end up having
                // the majority weight.
                foreach (var (edge, index) in edges.Select((x, i) => (x, i)))
                {
                    var fromDistance = distances[edge.From];
                    var toDistance = distances[edge.To];

                    if (Math.Abs(fromDistance - toDistance) == 1)
                    {
                        edgeWeights[index] += graph.Count - Math.Max(fromDistance, toDistance);
                    }
                }
            }

            return Enumerable.Zip(edges, edgeWeights)
                .Select(x => new EdgeWeight(x.First, x.Second));
        }

        /// <summary>
        /// Count how many distinct group there are in the graph, after we ignore certain edges.
        /// Return the size of each group.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="ignoreEdges">The paths in the graph which are ignored.</param>
        /// <returns>The number of nodes in each group.</returns>
        private static IEnumerable<int> GetGroupSizes(Dictionary<string, List<string>> graph,
            HashSet<Edge> ignoreEdges)
        {
            var allKeys = graph.Keys.ToHashSet();
            var foundKeys = new HashSet<string>();

            while (allKeys.Count != foundKeys.Count)
            {
                var nextKey = allKeys.Except(foundKeys)
                    .First();

                var queue = new Queue<string>();
                queue.Enqueue(nextKey);

                int groupSize = 0;
                while (queue.Count > 0)
                {
                    var key = queue.Dequeue();
                    if (foundKeys.Contains(key))
                    {
                        continue;
                    }

                    foundKeys.Add(key);
                    groupSize++;

                    foreach (var nextPath in graph[key])
                    {
                        if (!ignoreEdges.Contains(new Edge(key, nextPath)))
                        {
                            queue.Enqueue(nextPath);
                        }
                    }
                }

                yield return groupSize;
            }
        }

        /// <summary>
        /// Find three edges to remove from the graph to split it into two groups.
        /// Return the product of both group sizes.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The product of the two group sizes.</returns>
        private static int GetDisconnectGroups(string path)
        {
            var input = ReadInput(path);
            var edges = GetDistinctEdges(input);

            var edgeWeights = GetEdgeWeights(input, edges)
                .OrderByDescending(x => x.Weight);

            var bestEdges = edgeWeights
                .Select(x => x.Edge)
                .Take(3)
                .ToArray();

            var ignorePaths = new HashSet<Edge>()
            {
                bestEdges[0], new Edge(bestEdges[0].To, bestEdges[0].From),
                bestEdges[1], new Edge(bestEdges[1].To, bestEdges[1].From),
                bestEdges[2], new Edge(bestEdges[2].To, bestEdges[2].From)
            };
            var groupSizes = GetGroupSizes(input, ignorePaths)
                .ToArray();

            Assert.AreEqual(2, groupSizes.Length);

            return groupSizes[0] * groupSizes[1];
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample() => Assert.AreEqual(54, GetDisconnectGroups("AOC2023/Day25/Example.txt"));

        [TestMethod]
        public void SolveProblem() => Assert.AreEqual(543036, GetDisconnectGroups("AOC2023/Day25/Input.txt"));

        #endregion
    }
}

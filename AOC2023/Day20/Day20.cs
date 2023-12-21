using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AOC2023
{
    /// <summary>
    /// Solution for day 20:
    /// https://adventofcode.com/2023/day/20
    /// </summary>
    [TestClass]
    public class Day20
    {
        /// <summary>
        /// Stores a pulse type that can be triggered by a module and sent to the next.
        /// </summary>
        private enum PulseType
        {
            None,
            High,
            Low
        }

        /// <summary>
        /// A base module that can be classified into different types.
        /// </summary>
        private abstract class Module
        {
            /// <summary>
            /// Stores the destinations which are triggered when this module triggers.
            /// </summary>
            private readonly List<string> destinations = new();

            /// <summary>
            /// @Gets the destinations which are triggered when this module triggers.
            /// </summary>
            public List<string> Destinations => destinations;

            /// <summary>
            /// Add a destination to this module. This is a dependency which is triggered when this
            /// module trigggers.
            /// </summary>
            /// <param name="destination">The destination.</param>
            public void Add(string destination)
            {
                destinations.Add(destination);
            }

            /// <summary>
            /// Check whether this module maps to the destination mdoule.
            /// </summary>
            /// <param name="destination">The destination module.</param>
            /// <returns>True if the modules are connected.</returns>
            public bool MapsTo(string destination)
            {
                return destinations.Contains(destination);
            }

            /// <summary>
            /// Executes a pulse on this module by evaluating it, and then triggering the dependent modules.
            /// </summary>
            /// <param name="source">The source module.</param>
            /// <param name="pulseType">The pulse type sent by the source.</param>
            /// <returns>The pulses sent to the dependent modules.</returns>
            public IEnumerable<(string Module, PulseType PulseType)> Execute(string source, PulseType pulseType)
            {
                var result = Receive(source, pulseType);
                if (result == PulseType.None)
                {
                    yield break;
                }

                foreach (var destinationName in destinations)
                {
                    yield return (destinationName, result);
                }
            }

            /// <summary>
            /// Receive a pulse and evaluate it to return the pulse that is carried on.
            /// </summary>
            /// <param name="source">The source module sending the pulse.</param>
            /// <param name="pulseType">The type of pulse being sent.</param>
            /// <returns>The type of pulse being sent on.</returns>
            protected abstract PulseType Receive(string source, PulseType pulseType);

            /// <summary>
            /// Reset the module.
            /// </summary>
            public virtual void Reset() { }
        }

        private class FlipFlop : Module
        {
            private bool isOn = false;

            /// <summary>
            /// Reset the module.
            /// </summary>
            public override void Reset()
            {
                isOn = false;
            }

            /// <summary>
            /// Receive a pulse and evaluate it to return the pulse that is carried on.
            /// </summary>
            /// <param name="source">The source module sending the pulse.</param>
            /// <param name="pulseType">The type of pulse being sent.</param>
            /// <returns>The type of pulse being sent on.</returns>
            protected override PulseType Receive(string source, PulseType pulseType)
            {
                if (pulseType == PulseType.High)
                {
                    return PulseType.None;
                }
                else
                {
                    isOn = !isOn;

                    return isOn ? PulseType.High : PulseType.Low;
                }
            }
        }

        private class Conjunction : Module
        {
            private readonly Dictionary<string, PulseType> lastPulses = new();

            /// <summary>
            /// Add a source, which is a module that sends pulses to this one.
            /// </summary>
            /// <param name="source">The source.</param>
            public void AddSource(string source)
            {
                lastPulses[source] = PulseType.Low;
            }

            /// <summary>
            /// Reset the module.
            /// </summary>
            public override void Reset()
            {
                foreach (var key in lastPulses.Keys)
                {
                    lastPulses[key] = PulseType.Low;
                }
            }

            /// <summary>
            /// Receive a pulse and evaluate it to return the pulse that is carried on.
            /// </summary>
            /// <param name="source">The source module sending the pulse.</param>
            /// <param name="pulseType">The type of pulse being sent.</param>
            /// <returns>The type of pulse being sent on.</returns>
            protected override PulseType Receive(string source, PulseType pulseType)
            {
                lastPulses[source] = pulseType;

                if (lastPulses.Values.All(x => x == PulseType.High))
                {
                    return PulseType.Low;
                }
                else
                {
                    return PulseType.High;
                }
            }
        }

        private class Output : Module
        {
            /// <summary>
            /// Receive a pulse and evaluate it to return the pulse that is carried on.
            /// </summary>
            /// <param name="source">The source module sending the pulse.</param>
            /// <param name="pulseType">The type of pulse being sent.</param>
            /// <returns>The type of pulse being sent on.</returns>
            protected override PulseType Receive(string source, PulseType pulseType)
            {
                return PulseType.None;
            }
        }

        private class Broadcast : Module
        {
            /// <summary>
            /// Receive a pulse and evaluate it to return the pulse that is carried on.
            /// </summary>
            /// <param name="source">The source module sending the pulse.</param>
            /// <param name="pulseType">The type of pulse being sent.</param>
            /// <returns>The type of pulse being sent on.</returns>
            protected override PulseType Receive(string source, PulseType pulseType)
            {
                return pulseType;
            }
        }

        /// <summary>
        /// Stores a module configuration, and allows to execute it by pressing the button repeatedly.
        /// </summary>
        private class ModuleConfiguration
        {
            /// <summary>
            /// Stores the modules that make up this confguration.
            /// </summary>
            public Dictionary<string, Module> Modules
            {
                get;
            }

            /// <summary>
            /// Creates a new configuration instance.
            /// </summary>
            /// <param name="modules">The module.</param>
            public ModuleConfiguration(Dictionary<string, Module> modules)
            {
                this.Modules = modules;
            }

            /// <summary>
            /// The number of times the button was pressed.
            /// </summary>
            public int ButtonPresses
            {
                get;
                private set;
            }

            /// <summary>
            /// The low pulses which were generated.
            /// </summary>
            public int LowPulses
            {
                get;
                private set;
            }

            /// <summary>
            /// The high pulses which were generated.
            /// </summary>
            public int HighPulses
            {
                get;
                private set;
            }

            /// <summary>
            /// Execute a configuration of modules until we either reach the limit of button presses, or
            /// the output node receives a low pulse.
            /// </summary>
            /// <param name="maxButtonPresses">The limit on button presses.</param>
            /// <param name="outputNode">The output node which triggers termination.</param>
            public void Execute(int maxButtonPresses = int.MaxValue, string outputNode = "")
            {
                var startNode = "broadcaster";

                for (ButtonPresses = 1; ButtonPresses <= maxButtonPresses; ButtonPresses++)
                {
                    var queue = new Queue<(string Source, string Target, PulseType PulseType)>();
                    queue.Enqueue((startNode, startNode, PulseType.Low));

                    while (queue.Count != 0)
                    {
                        var (source, target, pulseType) = queue.Dequeue();

                        if (pulseType == PulseType.Low)
                        {
                            LowPulses++;
                        }
                        else
                        {
                            HighPulses++;
                        }

                        if (target == outputNode && pulseType == PulseType.Low)
                        {
                            return;
                        }

                        var targetModule = Modules[target];
                        foreach (var (nextTarget, nextPulse) in targetModule.Execute(source, pulseType))
                        {
                            queue.Enqueue((target, nextTarget, nextPulse));
                        }
                    }
                }
            }

            /// <summary>
            /// Resets the modules, so we can reset all state and execute the configuration
            /// again.
            /// </summary>
            public void Reset()
            {
                ButtonPresses = 0;

                LowPulses = 0;
                LowPulses = 0;

                foreach (var module in Modules.Values)
                {
                    module.Reset();
                }
            }
        }

        /// <summary>
        /// Reads the modules from the input file.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The modules.</returns>
        private static ModuleConfiguration ReadInput(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);

            var modules = new Dictionary<string, Module>();
            foreach (var line in lines)
            {
                var splitLine = line
                    .Replace(" -> ", ", ")
                    .Split(", ");

                string moduleName;
                Module module;
                if (splitLine[0][0] == '%')
                {
                    moduleName = splitLine[0][1..];
                    module = new FlipFlop();
                }
                else if (splitLine[0][0] == '&')
                {
                    moduleName = splitLine[0][1..];
                    module = new Conjunction();
                }
                else
                {
                    moduleName = splitLine[0];
                    module = new Broadcast();
                }

                for (int i = 1; i < splitLine.Length; i++)
                {
                    module.Add(splitLine[i]);
                }

                modules.Add(moduleName, module);
            }

            // We process the modules to find:
            // 1. Outputs which aren't defined as modules. We add special output modules for them.
            // 2. Conjunctions, where we need to add the reverse dependency.
            foreach (var (name, module) in modules.ToArray())
            {
                foreach (var destination in module.Destinations)
                {
                    if (!modules.TryGetValue(destination, out var source))
                    {
                        modules.Add(destination, new Output());
                    }
                    else if (source is Conjunction confunction)
                    {
                        confunction.AddSource(name);
                    }
                }
            }

            return new(modules);
        }

        /// <summary>
        /// Solves part 1 by counting the high and low pulses after executing 1000
        /// times and returning the product.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The product of the pulses.</returns>
        private static int CountLowHighPulses(string path)
        {
            var modules = ReadInput(path);

            modules.Execute(maxButtonPresses: 1000);

            return modules.LowPulses * modules.HighPulses;
        }

        /// <summary>
        /// The "rx" module consists of a set of submodules connected by
        /// a conjunction. Count the presses undil the rx button receives a
        /// low signal (meaning the subcomponents each must receive a high
        /// signal.
        /// </summary>
        /// <param name="path">The path to the input file.</param>
        /// <returns>The number of button presses.</returns>
        private static long CountPulseCycles(string path)
        {
            var configuration = ReadInput(path);
            var modules = configuration.Modules;

            var rx = modules["rx"];

            var conjunction = modules
                .Single(x => x.Value.MapsTo("rx"))
                .Key;
            
            var outputs = modules
                .Where(x => x.Value.MapsTo(conjunction))
                .Select(x => x.Key)
                .ToArray();

            var lcm = 1L;
            foreach (var output in outputs)
            {
                configuration.Execute(outputNode: output);
                lcm = MathUtil.LeastCommonMultiple(lcm, configuration.ButtonPresses);

                configuration.Reset();
            }

            return lcm;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(32000000, CountLowHighPulses("AOC2023/Day20/Example1.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(11687500, CountLowHighPulses("AOC2023/Day20/Example2.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(919383692, CountLowHighPulses("AOC2023/Day20/Input.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(247702167614647, CountPulseCycles("AOC2023/Day20/Input.txt"));

        #endregion
    }
}

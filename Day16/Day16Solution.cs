using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Solutions
{
    /// <summary>
    /// Solution for day 16:
    /// https://adventofcode.com/2021/day/16
    /// </summary>
    [SolutionClass(Day = 16)]
    public class Day16Solution
    {
        /// <summary>
        /// Stores the result after evaluating a bit pattern.
        /// </summary>
        /// <param name="VersionCount">The version count.</param>
        /// <param name="Value">The value of the bit pattern.</param>
        record Result(int VersionCount, long Value);

        /// <summary>
        /// A pattrn parser which takes a pattern and parses it, including
        /// sub-patterns.
        /// </summary>
        class PatternParser
        {
            /// <summary>
            /// Stores the pattern in binary format.
            /// </summary>
            private string pattern;

            /// <summary>
            /// The current position in the pattern.
            /// </summary>
            private int position = 0;

            /// <summary>
            /// Creates a new pattern parser, taking a hex string as input.
            /// </summary>
            /// <param name="pattern">The hex string.</param>
            internal PatternParser(string pattern)
            {
                this.pattern = ConvertHexToBinary(pattern);
            }

            /// <summary>
            /// Parses the pattern, and returns the result. In the case of sub-patterns,
            /// this method is recursively called.
            /// </summary>
            /// <returns>The result.</returns>
            internal Result ParsePattern()
            {
                int versionCount = ReadValue(3);
                int type = ReadValue(3);

                long value = 0;
                if (type == 4)
                {
                    value = ReadLongValue();
                }
                else
                {
                    List<long> values = new();

                    int lengthID = ReadValue(1);
                    if (lengthID == 0)
                    {
                        int length = ReadValue(15);

                        int tokenEnd = position + length;
                        while (position != tokenEnd)
                        {
                            var result = ParsePattern();

                            versionCount += result.VersionCount;
                            values.Add(result.Value);
                        }
                    }
                    else
                    {
                        int subPackets = ReadValue(11);

                        for (int i = 0; i < subPackets; i++)
                        {
                            var result = ParsePattern();

                            versionCount += result.VersionCount;
                            values.Add(result.Value);
                        }
                    }

                    switch (type)
                    {
                        case 0:
                            value = values.Sum();
                            break;

                        case 1:
                            value = values.Aggregate(1L, (x1, x2) => x1 * x2);
                            break;

                        case 2:
                            value = values.Min();
                            break;

                        case 3:
                            value = values.Max();
                            break;

                        case 5:
                            value = values[0] > values[1] ? 1 : 0;
                            break;

                        case 6:
                            value = values[0] < values[1] ? 1 : 0;
                            break;

                        case 7:
                            value = values[0] == values[1] ? 1 : 0;
                            break;
                    }
                }

                return new(versionCount, value);
            }

            /// <summary>
            /// Convert hexadecimal values to binary.
            /// </summary>
            /// <param name="hex">The hexadecimal string.</param>
            /// <returns>The binary string.</returns>
            private static string ConvertHexToBinary(string hex)
            {
                return String.Join(String.Empty,
                    hex.Select(
                        c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                    )
                );
            }

            /// <summary>
            /// Reads a long value from the pattern. A long value is groups
            /// of bits starting with 1, followed by a  group of bits starting
            /// with a 1.
            /// </summary>
            /// <returns>The value.</returns>
            private long ReadLongValue()
            {
                String valueString = "";

                bool isLast = false;
                while (!isLast)
                {
                    if (ReadValue(1) == 0)
                    {
                        isLast = true;
                    }

                    valueString += pattern.Substring(position, 4);
                    position += 4;
                }

                long value = Convert.ToInt64(valueString, 2);
                return value;
            }

            /// <summary>
            /// Reads a value from the input, and advances the position.
            /// </summary>
            /// <param name="count">The number of bits.</param>
            /// <returns>The value read.</returns>
            private int ReadValue(int count)
            {
                string valueString = pattern.Substring(position, count);
                int value = Convert.ToInt32(valueString, 2);

                position += count;

                return value;
            }
        }

        /// <summary>
        /// Counts the version numbers present in a hex pattern.
        /// </summary>
        /// <param name="pattern">The hex pattern.</param>
        /// <returns>The version number count.</returns>
        public int CountVersionNumbers(String pattern)
        {
            PatternParser parser = new(pattern);
            return parser.ParsePattern().VersionCount;
        }

        /// <summary>
        /// Solves a bit pattern and returns the value.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>The numeric value.</returns>
        public long DecodeBitPattern(String pattern)
        {
            
            PatternParser parser = new(pattern);
            return parser.ParsePattern().Value;
        }

        #region Solve Problems

        [SolutionMethod(Part = 1)]
        public int SolvePart1()
        {
            string line = System.IO.File.ReadAllText("Day16/Input.txt");
            return CountVersionNumbers(line);
        }

        [SolutionMethod(Part = 2)]
        public long SolvePart2()
        {
            string line = System.IO.File.ReadAllText("Day16/Input.txt");
            return DecodeBitPattern(line);
        }

        #endregion
    }
}

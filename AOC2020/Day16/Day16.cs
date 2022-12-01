using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    using Ticket = List<int>;

    /// <summary>
    /// Solution for day 16:
    /// https://adventofcode.com/2020/day/16
    /// </summary>
    [TestClass]
    public class Day16
    {
        /// <summary>
        /// A rule for a valid entry in a ticket. Has
        /// a name and two ranges that limit the number
        /// on a ticket.
        /// </summary>
        public class Rule
        {
            public string Name = "";

            public Tuple<int, int> Range1 = new(0, 0);

            public Tuple<int, int> Range2 = new(0, 0);
        }

        /// <summary>
        /// Reads a single rule from a string. For example,
        /// a rule is of the form:
        /// class: 26-579 or 585-963
        /// </summary>
        /// <param name="rule">The input string</param>
        /// <returns>The rule.</returns>
        private Rule ReadRule(String rule)
        {
            int nameEnd = rule.IndexOf(':');

            var name = rule.Substring(0, nameEnd);

            var range1Begin = nameEnd + 2;
            var range1End = rule.IndexOf(" or ");

            var range1Array = rule.Substring(range1Begin, range1End - range1Begin)
                .Split('-')
                .Select(str => int.Parse(str))
                .ToArray();
            var range1 = new Tuple<int, int>(range1Array[0], range1Array[1]);

            var range2Begin = range1End + 4;
            var range2Array = rule.Substring(range2Begin)
                .Split('-')
                .Select(str => int.Parse(str))
                .ToArray();
            var range2 = new Tuple<int, int>(range2Array[0], range2Array[1]);

           return new Rule()
           {
               Name = name,
               Range1 = range1,
               Range2 = range2
           };
        }

        /// <summary>
        /// Stores the ticket info. Has a bunch of rules,
        /// and a bunch of tickets. The first ticket
        /// is your own.
        /// </summary>
        public class TicketInfo
        {
            public List<Rule> Rules = new();

            public List<Ticket> Tickets = new();
        }

        /// <summary>
        /// Read the ticket info from a file.
        /// </summary>
        /// <param name="path">The input path</param>
        /// <returns>The ticket info</returns>
        public TicketInfo ReadTicketInfo(String path)
        {
            var input = System.IO.File.ReadLines(path);

            var ticketInfo = new TicketInfo()
            {
                Rules = new List<Rule>(),
                Tickets = new List<Ticket>()
            };

            foreach (var line in input)
            {
                if (line == "" ||
                    line == "your ticket:" ||
                    line == "nearby tickets:")
                {
                    continue;
                }

                if (char.IsDigit(line[0]))
                {
                    var ticketValues = line.Split(',')
                        .Select(str => int.Parse(str));

                    ticketInfo.Tickets.Add(ticketValues.ToList());
                }
                else
                {
                    ticketInfo.Rules.Add(ReadRule(line));
                }
            }

            return ticketInfo;
        }

        /// <summary>
        /// Checks whether a given value is valid for a rule.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>True if the value is valid.</returns>
        public bool IsValid(int value, Rule rule)
        {
            return (value >= rule.Range1.Item1 && value <= rule.Range1.Item2) ||
                   (value >= rule.Range2.Item1 && value <= rule.Range2.Item2);
        }

        /// <summary>
        /// Checks whether a given value is valid for any rule.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="rules">The rules.</param>
        /// <returns>True if the value is valid.</returns>
        public bool IsValid(int value, IEnumerable<Rule> rules)
        {
            return rules.Any(rule => IsValid(value, rule));
        }

        /// <summary>
        /// Checks whether a sequence of values is valid for
        /// a rule.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>True if the values are alll valid.</returns>
        public bool IsValid(IEnumerable<int> values, Rule rule)
        {
            var invalidValues = values.Where(v => !IsValid(v, rule));
            return !invalidValues.Any();
        }

        /// <summary>
        /// Checks whether given values are all valid for any rule.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="rules">The rules.</param>
        /// <returns>True if the values are valid.</returns>
        public bool IsValid(IEnumerable<int> values, IEnumerable<Rule> rules)
        {
            var invalidValues = values.Where(v => !IsValid(v, rules));
            return !invalidValues.Any();
        }

        /// <summary>
        /// Reads the ticket info from a file, and returns the sum
        /// of all invalid entries.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The error rate.</returns>
        public int GetSolution1(String path)
        {
            var ticketInfo = ReadTicketInfo(path);

            int error_rate = 0;
            foreach (var ticket in ticketInfo.Tickets)
            {
                foreach (var ticketNumber in ticket)
                {
                    if (!IsValid(ticketNumber, ticketInfo.Rules))
                    {
                        error_rate += ticketNumber;
                    }
                }
            }

            return error_rate;
        }

        /// <summary>
        /// Gets the matching rule for each column.
        /// </summary>
        /// <param name="ticketInfo">The ticket info.</param>
        /// <returns>The matching rule for each column.</returns>
        public List<int> GetRulesForColumns(TicketInfo ticketInfo)
        {
            var ruleForColumn = new int[ticketInfo.Rules.Count];

            // We keep track of unmatched rules and columns. As soon as a
            // match between the two is found, we remove them.

            var unmatchedRules = Enumerable.Range(0, ticketInfo.Rules.Count)
                .ToList();
            var unmatchedColumns = Enumerable.Range(0, ticketInfo.Rules.Count)
                .ToList();

            // We iterate until we've matched all rules.
            while (unmatchedRules.Count != 0)
            {
                foreach (var unmatchedColumn in unmatchedColumns)
                {
                    // We need to filter out invalid tickets.
                    var values = ticketInfo.Tickets
                        .Where(ticket => IsValid(ticket, ticketInfo.Rules))
                        .Select(ticket => ticket[unmatchedColumn])
                        .ToList();

                    var validRules = new List<int>();

                    // Find each unmatched rule that is valid for the values.
                    foreach (var unmatchedRule in unmatchedRules)
                    {
                        if (IsValid(values, ticketInfo.Rules[unmatchedRule]))
                        {
                            validRules.Add(unmatchedRule);
                        }
                    }

                    // If there is only one unique result, then we have
                    // found a valid match.
                    if (validRules.Count == 1)
                    {
                        var validRule = validRules[0];

                        unmatchedRules.Remove(validRule);
                        unmatchedColumns.Remove(unmatchedColumn);

                        ruleForColumn[unmatchedColumn] = validRule;

                        break;
                    }
                }
            }

            return ruleForColumn.ToList();
        }

        /// <summary>
        /// Read the ticket info, and return the ticket
        /// values ordered by the rules.
        /// </summary>
        /// <param name="path">The input path.</param>
        /// <returns>The ordered ticket.</returns>
        public Ticket GetExample2(String path)
        {
            var ticketInfo = ReadTicketInfo(path);
            var rulesForColumn = GetRulesForColumns(ticketInfo);

            var orderedTicket = new Ticket();
            for (int i = 0; i < ticketInfo.Rules.Count; ++i)
            {
                var column = rulesForColumn.IndexOf(i);

                orderedTicket.Add(ticketInfo.Tickets[0][column]);
            }

            return orderedTicket;
        }

        /// <summary>
        /// Reads the ticket info, and returns the product of
        /// all rules starting with "departure"
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The product.</returns>
        public long GetSolution2(String path)
        {
            var ticketInfo = ReadTicketInfo(path);
            var rulesForColumn = GetRulesForColumns(ticketInfo);

            long departureProduct = 1;
            for (int i = 0; i < ticketInfo.Rules.Count; ++i)
            {
                var rule = ticketInfo.Rules[i];

                if (rule.Name.StartsWith("departure"))
                {
                    var column = rulesForColumn.IndexOf(i);

                    departureProduct *= ticketInfo.Tickets[0][column];
                }
            }

            return departureProduct;
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(71, GetSolution1("AOC2020/Day16/Example1.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(19070, GetSolution1("AOC2020/Day16/Input.txt"));

        [TestMethod]
        public void SolveExample2()
        {
            var result = GetExample2("AOC2020/Day16/Example2.txt");

            Assert.AreEqual(12, result[0]);
            Assert.AreEqual(11, result[1]);
            Assert.AreEqual(13, result[2]);
        }

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(161926544831, GetSolution2("AOC2020/Day16/Input.txt"));

        #endregion
    }
}

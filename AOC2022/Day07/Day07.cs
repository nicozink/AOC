using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace AOC2022
{
    /// <summary>
    /// Solution for day 7:
    /// https://adventofcode.com/2022/day/7
    /// </summary>
    [TestClass]
    public class Day07
    {
        /// <summary>
        /// Stores a file in a filesystem.
        /// </summary>
        private class File
        {
            /// <summary>
            /// The name of the file.
            /// </summary>
            public string Name { get; set; } = "";

            /// <summary>
            /// The size of the file.
            /// </summary>
            public int Size { get; set; } = 0;
        }

        /// <summary>
        /// Stores a folder in the file system.
        /// </summary>
        private class Folder : File
        {
            public List<File> Nodes { get; } = new();
        }

        /// <summary>
        /// Parses the input commands, and constructs the underlying file
        /// system.
        /// </summary>
        /// <param name="path">The path to the commands.</param>
        /// <returns>The file system.</returns>
        private static Folder ReadInput(string path)
        {
            Folder root = new()
            {
                Name = "/"
            };
            Folder current = root;
            Stack<Folder> parents = new();
            
            var lines = System.IO.File.ReadLines(path);
            foreach (var line in lines)
            {
                if (line == "$ cd /")
                {
                    current = root;
                    parents = new();
                }
                else if (line == "$ cd ..")
                {
                    current = parents.Pop();
                }
                else if (line.StartsWith("$ cd "))
                {
                    parents.Push(current);

                    var name = line[5..];
                    var folder = current.Nodes.First(x => x.Name == name) as Folder;
                    current = folder!;
                }
                else if (line == "$ ls")
                {
                    // Print folders/files
                }
                else if (line.StartsWith("dir "))
                {
                    var folder = new Folder()
                    {
                        Name = line[4..]
                    };

                    current.Nodes.Add(folder);
                }
                else
                {
                    var splitSizeName = line.Split();
                    var file = new File()
                    {
                        Name = splitSizeName[1],
                        Size = int.Parse(splitSizeName[0])
                    };

                    current.Size += file.Size;
                    foreach (var parent in parents)
                    {
                        parent.Size += file.Size;
                    }

                    current.Nodes.Add(file);
                }
            }

            return root;
        }

        /// <summary>
        /// Gets a recursive list of folders from the file system root (including
        /// the root).
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns>The folders.</returns>
        private static IEnumerable<Folder> GetFoldersRecursive(Folder root)
        {
            yield return root;

            foreach (var child in root.Nodes.OfType<Folder>())
            {
                foreach (var subFolder in GetFoldersRecursive(child))
                {
                    yield return subFolder;
                }
            }
        }

        /// <summary>
        /// Gets all folders where the size is less than 100k.
        /// </summary>
        /// <param name="path">The input file giving the file system.</param>
        /// <returns>The total size of the folders.</returns>
        private static int GetFolderSizessLess100k(string path)
        {
            var root = ReadInput(path);
            return GetFoldersRecursive(root)
                .Where(x => x.Size < 100000)
                .Sum(x => x.Size);
        }

        /// <summary>
        /// Find the size of the smallest folder to delete to bring the
        /// size down to the desired limit.
        /// </summary>
        /// <param name="path">The input file giving the file system.</param>
        /// <returns>The size of the folder to delete.</returns>
        private static int FindSmallesSizeToDelete(string path)
        {
            var root = ReadInput(path);

            var sizeLimit = 40000000;
            var sizeToDelete = root.Size - sizeLimit;

            return GetFoldersRecursive(root)
                .Where(x => x.Size > sizeToDelete)
                .Min(x => x.Size);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(95437, GetFolderSizessLess100k("AOC2022/Day07/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(1644735, GetFolderSizessLess100k("AOC2022/Day07/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual(24933642, FindSmallesSizeToDelete("AOC2022/Day07/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual(1300850, FindSmallesSizeToDelete("AOC2022/Day07/Input.txt"));

        #endregion
    }
}

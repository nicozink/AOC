using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2020
{
    /// <summary>
    /// Solution for day 21:
    /// https://adventofcode.com/2020/day/21
    /// </summary>
    [TestClass]
    public class Day21
    {
        /// <summary>
        /// Stores a food item. This contains a list of ingredients,
        /// and a list of allergens.
        /// </summary>
        class FoodItem
        {
            internal HashSet<string> Ingredients = new HashSet<string>();

            internal HashSet<string> Allergens = new HashSet<string>();
        }

        /// <summary>
        /// Reads the foods from the file.
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>The food items</returns>
        private IEnumerable<FoodItem> ReadFoods(String path)
        {
            var input = System.IO.File.ReadLines(path);

            foreach (var line in input)
            {
                var ingredientsAllergens = line
                    .Replace(")", "")
                    .Split(new string[] { " (contains " }, StringSplitOptions.None);

                var ingredients = ingredientsAllergens[0]
                    .Split()
                    .ToHashSet();

                var allergens = ingredientsAllergens[1]
                    .Split(new string[] { ", " }, StringSplitOptions.None)
                    .ToHashSet();

                yield return new FoodItem()
                {
                    Ingredients = ingredients,
                    Allergens = allergens
                };
            }
        }

        /// <summary>
        /// Gets a lookup of the ingredients based on the allergen.
        /// </summary>
        /// <param name="foodItems">The food items.</param>
        /// <returns>The lookup of possible ingredients for each allergen.</returns>
        Dictionary<string, HashSet<string>> GetIngredientLookup(IEnumerable<FoodItem> foodItems)
        {
            var uniqueAllergens = new HashSet<string>(foodItems.SelectMany(x => x.Allergens));

            var ingredientLoopup = new Dictionary<string, HashSet<string>>();
            foreach (var allergen in uniqueAllergens)
            {
                HashSet<string>? possibleIngredients = null;

                // We go through each food where the allergen contains the specific food.
                // Where there are more than one food containing the allergen, we take the
                // intersection of the hash sets to filter the list of ingredients.
                var allergenFoods = foodItems.Where(item => item.Allergens.Contains(allergen));
                foreach (var food in allergenFoods)
                {
                    // If the possible ingredients is null, we take the first list
                    // of ingredients we find to seed the list. Otherwise, we combine
                    // the new list of ingredients with the previous one.
                    if (possibleIngredients == null)
                    {
                        possibleIngredients = food.Ingredients.ToHashSet();
                    }
                    else
                    {
                        possibleIngredients.IntersectWith(food.Ingredients);
                    }
                }

                ingredientLoopup.Add(allergen, possibleIngredients!);
            }

            return ingredientLoopup;
        }

        /// <summary>
        /// This function creates the lookup, and then counts the number of times that
        /// ingredients don't contain any allergens.
        /// </summary>
        /// <param name="path">The input path.</param>
        /// <returns>The number of times that ingredients don't add any allergens.</returns>
        public long GetSolution1(String path)
        {
            var foodItems = ReadFoods(path);
            var ingredientLoopup = GetIngredientLookup(foodItems);

            var uniqueIngredients = new HashSet<string>(foodItems.SelectMany(x => x.Ingredients));

            var unusedCount = 0;
            foreach (var ingredient in uniqueIngredients)
            {
                // If ingredients aren't present for any of the allergens, then
                // we add the number of times this ingredient is present to the
                // total count.
                if (!ingredientLoopup.Any(x => x.Value.Contains(ingredient)))
                {
                    unusedCount += foodItems.Count(item => item.Ingredients.Contains(ingredient));
                }
            }

            return unusedCount;
        }

        /// <summary>
        /// This figures out a pairing between the allergens and ingredients, and produces
        /// a list of ingredients based on the sorted list of allergens.
        /// </summary>
        /// <param name="path">The input file.</param>
        /// <returns>The list of ingredients.</returns>
        public string GetSolution2(String path)
        {
            var foodItems = ReadFoods(path);
            var ingredientLoopup = GetIngredientLookup(foodItems);

            // In this loop, we repeatedly look at the ingredient lookup.
            // and ingredient that is matched to exactly one allergen is
            // considered "locked" in, so we look for other allergens and
            // remove locked in ingredients from their list of possibilities.
            while (ingredientLoopup.Any(x => x.Value.Count != 1))
            {
                // These are all allergens that still have multiple
                // options for an ingredient.
                var manyAllergens = ingredientLoopup
                    .Where(x => x.Value.Count != 1)
                    .ToList();

                foreach (var lookup in manyAllergens)
                {
                    // We go through each ingredient in the list.
                    foreach (var ingredient in lookup.Value.ToList())
                    {
                        // We look to see if any ingredient in this list
                        // is present as a locked in ingredient for another
                        // allergen.
                        var hasOtherUniqueUse = ingredientLoopup
                            .Any(x => x.Key != lookup.Key &&
                                x.Value.Count == 1 &&
                                x.Value.Contains(ingredient));

                        // If it is, we remove it from the list.
                        if (hasOtherUniqueUse)
                        {
                            lookup.Value.Remove(ingredient);
                        }
                    }
                }
            }

            // We now have a one-to-one mapping between ingredients and
            // allergens. So we sort by allergen, and create a list of ingredients.

            var uniqueList = ingredientLoopup
                .OrderBy(x => x.Key)
                .Select(x => x.Value.First());

            var returnString = "";
            foreach (var item in uniqueList)
            {
                returnString += item + ",";
            }

            return returnString.Substring(0, returnString.Length - 1);
        }

        #region Solve Problems

        [TestMethod]
        public void SolveExample1() => Assert.AreEqual(5, GetSolution1("AOC2020/Day21/Example.txt"));

        [TestMethod]
        public void SolvePart1() => Assert.AreEqual(2584, GetSolution1("AOC2020/Day21/Input.txt"));

        [TestMethod]
        public void SolveExample2() => Assert.AreEqual("mxmxvkd,sqjhc,fvjkl", GetSolution2("AOC2020/Day21/Example.txt"));

        [TestMethod]
        public void SolvePart2() => Assert.AreEqual("fqhpsl,zxncg,clzpsl,zbbnj,jkgbvlxh,dzqc,ppj,glzb", GetSolution2("AOC2020/Day21/Input.txt"));

        #endregion
    }
}

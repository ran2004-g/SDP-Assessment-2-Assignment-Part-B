using System.Collections.Generic;
using RecipeManagement.Core;
using Xunit;

namespace RecipeManagement.Tests;

public sealed class RecipeManagerPartBTests
{
    private static RecipeManager CreateTestManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 1,
                Title = "Chicken Salad",
                Ingredients = new List<string> { "chicken breast", "lettuce", "olive oil" },
                Instructions = new List<string> { "chop", "mix" },
                Nutrition = new NutritionInfo { ProteinG = 35.0 }
            },
            new Recipe
            {
                Id = 2,
                Title = "Beef Steak",
                Ingredients = new List<string> { "beef", "salt", "pepper" },
                Instructions = new List<string> { "season", "fry" },
                Nutrition = new NutritionInfo { ProteinG = 50.0 }
            },
            new Recipe
            {
                Id = 3,
                Title = "Vegetable Soup",
                Ingredients = new List<string> { "carrot", "potato", "salt" },
                Instructions = new List<string> { "chop", "boil" }
            }
        });
    }
    [Fact]
    public void SearchByTitle_PartialMatchCaseInsensitive_ReturnsMatches()
    {
        var manager = CreateTestManager();
        var results = manager.SearchByTitle("chicken");
        
        Assert.Single(results);
        Assert.Equal("Chicken Salad", results[0].Title);
    }

    [Fact]
    public void SearchByTitle_BlankSearch_ReturnsAllRecipes()
    {
        var manager = CreateTestManager();
        var results = manager.SearchByTitle("");
        
        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void SearchByIngredient_MatchInIngredientList_ReturnsRecipeOnce()
    {
        var manager = CreateTestManager();
        var results = manager.SearchByIngredient("salt");
        
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void GetHighestProteinRecipes_ValidCount_ReturnsSortedTopN()
    {
        var manager = CreateTestManager();
        var results = manager.GetHighestProteinRecipes(2);
        
        Assert.Equal(2, results.Count);
        Assert.Equal("Beef Steak", results[0].Title);
        Assert.Equal("Chicken Salad", results[1].Title);
    }

    
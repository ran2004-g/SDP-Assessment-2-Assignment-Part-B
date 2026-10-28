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

    [Fact]
    public void GetHighestProteinRecipes_ZeroOrNegativeCount_ReturnsEmpty()
    {
        var manager = CreateTestManager();
        Assert.Empty(manager.GetHighestProteinRecipes(0));
        Assert.Empty(manager.GetHighestProteinRecipes(-5));
    }

    [Fact]
    public void SavedRecipe_AddDuplicate_ReturnsFalse()
    {
        var manager = CreateTestManager();
        bool first = manager.AddSavedRecipe(1);
        bool second = manager.AddSavedRecipe(1);
        
        Assert.True(first);
        Assert.False(second);
        Assert.Single(manager.GetSavedRecipes());
    }

    [Fact]
    public void SavedRecipe_AddNonExistentId_ReturnsFalse()
    {
        var manager = CreateTestManager();
        bool result = manager.AddSavedRecipe(999);
        
        Assert.False(result);
    }

    [Fact]
    public void SavedRecipe_RemoveAndCheck_WorksCorrectly()
    {
        var manager = CreateTestManager();
        manager.AddSavedRecipe(1);
        
        Assert.True(manager.IsRecipeSaved(1));
        Assert.True(manager.RemoveSavedRecipe(1));
        Assert.False(manager.IsRecipeSaved(1));
    }

    [Fact]
    public void IntegratedWorkflow_SearchSaveRemove_PartAStillWorks()
    {
        var manager = CreateTestManager();
        
       
        var searchResults = manager.SearchByTitle("beef");
        Assert.Single(searchResults);
        
        bool saved = manager.AddSavedRecipe(searchResults[0].Id);
        Assert.True(saved);
        Assert.True(manager.IsRecipeSaved(searchResults[0].Id));
        
        bool removed = manager.RemoveSavedRecipe(searchResults[0].Id);
        Assert.True(removed);
        Assert.Empty(manager.GetSavedRecipes());
        
        
        Assert.Equal(3, manager.RecipeCount);
        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.Single(manager.GetCookingPlan());
    }
}
using System.Collections.Generic;
using RecipeManagement.Core;
using Xunit;
namespace RecipeManagement.Tests;
/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }

    [Fact]
    public void AddRecipe_NormalValidRecipe_AddSuccessReturnsTrue()
    {
        var mgr = new RecipeManager(new List<Recipe>());
        var recipe = new Recipe
        {
            Id = 5,
            Title = "Pasta",
            Ingredients = new List<string>(),
            Instructions = new List<string>()
        };
        bool result = mgr.AddRecipe(recipe);
        Assert.True(result);
        Assert.Equal(1, mgr.RecipeCount);
    }

    [Fact]
    public void AddRecipe_DuplicateId_ReturnsFalse()
    {
        var mgr = CreateManager();
        var duplicate = new Recipe
        {
            Id = 10,
            Title = "Duplicate",
            Ingredients = new List<string>(),
            Instructions = new List<string>()
        };
        bool result = mgr.AddRecipe(duplicate);
        Assert.False(result);
        Assert.Equal(2, mgr.RecipeCount);
    }

    [Fact]
    public void FindRecipe_MissingId_ReturnsNull()
    {
        var manager = CreateManager();
        var found = manager.FindRecipe(999);
        Assert.Null(found);
    }

    [Fact]
    public void RemoveRecipe_ExistingRecipe_RemoveSuccess()
    {
        var manager = CreateManager();
        bool removed = manager.RemoveRecipe(10);
        Assert.True(removed);
        Assert.Null(manager.FindRecipe(10));
        Assert.Equal(1,manager.RecipeCount);

    }
    [Fact]
     public void RemoveRecipe_WhenInCookingPlan_ReturnsFalse()
    {
        var manager = CreateManager();
        // Add recipe 10 into cooking plan
        manager.AddRecipeToCookingPlan(10);
        // Attempt to remove recipe 10 from catalogue while it exists in cooking plan
        bool removeResult = manager.RemoveRecipe(10);
        var foundRecipe = manager.FindRecipe(10);

        Assert.False(removeResult);
        Assert.NotNull(foundRecipe);
    }


    [Fact]
    public void ShoppingList_AddIngredients_AddedInOrder()
    {
        var manager = CreateManager();
        // Add all ingredients of recipe 10 to shopping list
        int added = manager.AddIngredientsToShoppingList(10);
        var shoppingItems = manager.GetShoppingList();
        // Verify ingredients order, Recipe A has ingredient "1 apple"
        Assert.Equal(1, added);
        Assert.Single(shoppingItems);
        Assert.Equal("1 apple", shoppingItems[0]);
    }

    [Fact]
     public void ShoppingList_AddTwoRecipes_IngredientsConcatenated()
   {
       var manager = CreateManager();
       // Recipe10 contains "1 apple"; manually add ingredients to Recipe20 for testing
       var recipe20 = manager.FindRecipe(20)!;
       recipe20.Ingredients.Add("2 eggs");

       int added1 = manager.AddIngredientsToShoppingList(10);
       int added2 = manager.AddIngredientsToShoppingList(20);
       var shoppingItems = manager.GetShoppingList();

       Assert.Equal(1, added1);
       Assert.Equal(1, added2);
       Assert.Equal(2, shoppingItems.Count);
       Assert.Equal("1 apple", shoppingItems[0]);
       Assert.Equal("2 eggs", shoppingItems[1]);
   }

    [Fact]
     public void ShoppingList_ClearShoppingList_EmptiesList()
   {
       var manager = CreateManager();
       // Add recipe 10 ingredients into shopping list
       manager.AddIngredientsToShoppingList(10);
       manager.ClearShoppingList();
       var shoppingItems = manager.GetShoppingList();
       Assert.Empty(shoppingItems);
   }
   [Fact]
    public void StartCooking_InvalidRecipeId_ReturnsFalse()
   {
       var manager = CreateManager();
       // Attempt to start cooking for a recipe that does not exist
       bool startResult = manager.StartCooking(999);
       Assert.False(startResult);
   }
   [Fact]
    public void CompleteNextInstruction_NoActiveRecipe_ReturnsNull()
   {
       var manager = CreateManager();
       // No recipe has been started, so there are no active instructions
       var step = manager.CompleteNextInstruction();
       Assert.Null(step);
   }
   [Fact]
    public void PeekLastRemovedRecipe_EmptyStack_ReturnsNull()
   {
       var manager = CreateManager();
       // Stack of removed recipes is empty, peek should return null
       var lastRemoved = manager.PeekLastRemovedRecipe();
       Assert.Null(lastRemoved);
   }

   [Fact]
    public void PeekNextInstruction_NoActiveRecipe_ReturnsNull()
   {
       var manager = CreateManager();
       // No recipe has been started, so cannot peek next instruction
       var step = manager.PeekNextInstruction();
       Assert.Null(step);
   }
   [Fact]
    public void CompleteNextInstruction_AllStepsFinished_ReturnsNull()
   {
       var manager = CreateManager();
       // Start recipe 10 and consume all instructions
       manager.StartCooking(10);
       manager.CompleteNextInstruction();
       manager.CompleteNextInstruction();
       // No more instructions left
       var nextStep = manager.CompleteNextInstruction();
       Assert.Null(nextStep);
   }

   [Fact]
    public void RemoveRecipeFromCookingPlan_RecipeNotInPlan_ReturnsFalse()
   {
       var manager = CreateManager();
       // Try to remove a recipe id that is not present in cooking plan
       bool removed = manager.RemoveRecipeFromCookingPlan(999);
       Assert.False(removed);
   }
   [Fact]
    public void RestoreLastRemovedRecipe_NothingRemoved_ReturnsFalse()
   {
       var manager = CreateManager();
       // No recipes have been removed from cooking plan yet
       bool restoreResult = manager.RestoreLastRemovedRecipe();
       Assert.False(restoreResult);
   }

   [Fact]
    public void GetCookingPlan_EmptyByDefault_ReturnsEmptyList()
   {
       var manager = CreateManager();
       // Cooking plan should be empty when newly created
       var plan = manager.GetCookingPlan();
       Assert.Empty(plan);
   }
  [Fact]
   public void GetCookingPlan_ItemsInAddedOrder_ReturnsPreservedOrder()
  {
      var manager = CreateManager();
      // Add recipe 10 then 20 into cooking plan
      manager.AddRecipeToCookingPlan(10);
      manager.AddRecipeToCookingPlan(20);
      var plan = manager.GetCookingPlan();
      Assert.Equal(new[] {10, 20}, plan);
  }
  [Fact]
   public void CookingPlan_DuplicateRecipe_ReturnsFalse()
  {
      var manager = CreateManager();
      // Add recipe 10 to cooking plan for the first time
      bool firstAdd = manager.AddRecipeToCookingPlan(10);
      // Try adding recipe 10 again
      bool secondAdd = manager.AddRecipeToCookingPlan(10);
      var plan = manager.GetCookingPlan();

      Assert.True(firstAdd);
      Assert.False(secondAdd);
      Assert.Single(plan);
      Assert.Equal(10, plan[0]);
  }



    


}

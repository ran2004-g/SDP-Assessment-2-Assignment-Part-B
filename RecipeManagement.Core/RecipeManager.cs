
using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManagement.Core;

public sealed class RecipeManager : IRecipeManager
{
    // Part A: Collection fields
    private readonly Dictionary<int, Recipe> _recipeCatalogue;
    private readonly List<string> _shoppingList;
    private readonly LinkedList<int> _cookingPlan;
    private readonly Stack<int> _removedRecipeHistory;
    private readonly Queue<string> _activeInstructions;

    // Part B: Saved recipes
    private readonly HashSet<int> _savedRecipes;

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes == null)
            throw new ArgumentNullException(nameof(recipes));

        _recipeCatalogue = new Dictionary<int, Recipe>();
        _shoppingList = new List<string>();
        _cookingPlan = new LinkedList<int>();
        _removedRecipeHistory = new Stack<int>();
        _activeInstructions = new Queue<string>();
        _savedRecipes = new HashSet<int>();

        foreach (var recipe in recipes)
        {
            if (recipe == null)
                throw new ArgumentException("Recipe cannot be null.");

            if (!AddRecipe(recipe))
            {
                throw new ArgumentException(
                    $"Invalid or duplicate recipe ID: {recipe.Id}");
            }
        }
    }

    // Collection count properties
    public int RecipeCount => _recipeCatalogue.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _activeInstructions.Count;
    public int RemovedRecipeCount => _removedRecipeHistory.Count;

    // Check if a recipe is already in the cooking plan
    private bool IsRecipeInCookingPlan(int recipeId)
    {
        foreach (int id in _cookingPlan)
        {
            if (id == recipeId)
                return true;
        }

        return false;
    }

    // ================= PART A =================

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null)
            throw new ArgumentNullException(nameof(recipe));

        if (recipe.Id <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(recipe.Title))
            return false;

        if (_recipeCatalogue.ContainsKey(recipe.Id))
            return false;

        _recipeCatalogue.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        _recipeCatalogue.TryGetValue(recipeId, out var recipe);
        return recipe;
    }

    
public bool RemoveRecipe(int recipeId)
{
    if (!_recipeCatalogue.ContainsKey(recipeId))
        return false;

    if (IsRecipeInCookingPlan(recipeId))
        return false;

    _recipeCatalogue.Remove(recipeId);

    // Remove the recipe from saved recipes as well.
    _savedRecipes.Remove(recipeId);

    return true;
}


    public int AddIngredientsToShoppingList(int recipeId)
    {
        var recipe = FindRecipe(recipeId);

        if (recipe == null)
            return 0;

        int count = 0;

        foreach (var ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
            count++;
        }

        return count;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return _shoppingList.AsReadOnly();
    }

    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!_recipeCatalogue.ContainsKey(recipeId))
            return false;

        if (IsRecipeInCookingPlan(recipeId))
            return false;

        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        var current = _cookingPlan.First;

        while (current != null)
        {
            if (current.Value == recipeId)
            {
                _cookingPlan.Remove(current);
                _removedRecipeHistory.Push(recipeId);
                return true;
            }

            current = current.Next;
        }

        return false;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (_removedRecipeHistory.Count == 0)
            return null;

        return _removedRecipeHistory.Peek();
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipeHistory.Count == 0)
            return false;

        int id = _removedRecipeHistory.Pop();

        if (_recipeCatalogue.ContainsKey(id) &&
            !IsRecipeInCookingPlan(id))
        {
            _cookingPlan.AddLast(id);
            return true;
        }

        return false;
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        var result = new List<int>();

        foreach (int id in _cookingPlan)
        {
            result.Add(id);
        }

        return result.AsReadOnly();
    }

    public bool StartCooking(int recipeId)
    {
        var recipe = FindRecipe(recipeId);

        if (recipe == null)
            return false;

        if (recipe.Instructions.Count == 0)
            return false;

        _activeInstructions.Clear();

        foreach (var instruction in recipe.Instructions)
        {
            _activeInstructions.Enqueue(instruction);
        }

        return true;
    }

    public string? PeekNextInstruction()
    {
        if (_activeInstructions.Count == 0)
            return null;

        return _activeInstructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (_activeInstructions.Count == 0)
            return null;

        return _activeInstructions.Dequeue();
    }

    // ================= PART B =================

    // Search recipes by title
    public IReadOnlyList<Recipe> SearchByTitle(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return _recipeCatalogue.Values
                .ToList()
                .AsReadOnly();
        }

        return _recipeCatalogue.Values
            .Where(recipe =>
                recipe.Title.Contains(
                    searchText,
                    StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }

    // Search recipes by ingredient
    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return _recipeCatalogue.Values
                .ToList()
                .AsReadOnly();
        }

        return _recipeCatalogue.Values
            .Where(recipe =>
                recipe.Ingredients.Any(ingredient =>
                    ingredient.Contains(
                        searchText,
                        StringComparison.OrdinalIgnoreCase)))
            .ToList()
            .AsReadOnly();
    }

    // Get recipes with highest protein content
    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count)
    {
        if (count <= 0)
        {
            return new List<Recipe>().AsReadOnly();
        }

        return _recipeCatalogue.Values
            .Where(recipe =>
                recipe.Nutrition?.ProteinG.HasValue == true)
            .OrderByDescending(recipe =>
                recipe.Nutrition!.ProteinG!.Value)
            .Take(count)
            .ToList()
            .AsReadOnly();
    }

    // Add recipe to saved recipes
    public bool AddSavedRecipe(int recipeId)
    {
        if (!_recipeCatalogue.ContainsKey(recipeId))
            return false;

        return _savedRecipes.Add(recipeId);
    }

    // Remove recipe from saved recipes
    public bool RemoveSavedRecipe(int recipeId)
    {
        return _savedRecipes.Remove(recipeId);
    }

    // Check whether recipe is saved
    public bool IsRecipeSaved(int recipeId)
    {
        return _savedRecipes.Contains(recipeId);
    }

    // Return saved recipe IDs
    public IReadOnlyList<int> GetSavedRecipes()
    {
        return _savedRecipes.ToList().AsReadOnly();
    }
}

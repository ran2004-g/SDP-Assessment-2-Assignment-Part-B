using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // Part A: private collection fields
    private readonly Dictionary<int, Recipe> _recipeCatalogue;
    private readonly List<string> _shoppingList;
    private readonly LinkedList<int> _cookingPlan;
    private readonly Stack<int> _removedRecipeHistory;
    private readonly Queue<string> _activeInstructions;

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // Validate input: null recipes are not allowed
        if (recipes == null)
            throw new ArgumentNullException(nameof(recipes));

        // Initialise all collections
        _recipeCatalogue = new Dictionary<int, Recipe>();
        _shoppingList = new List<string>();
        _cookingPlan = new LinkedList<int>();
        _removedRecipeHistory = new Stack<int>();
        _activeInstructions = new Queue<string>();

        // Add recipes to the catalogue and validate duplicate IDs,
        // non-positive IDs, and blank titles
        foreach (var r in recipes)
        {
            bool ok = AddRecipe(r);

            if (!ok)
            {
                throw new ArgumentException(
                    $"Invalid recipe id {r.Id} or duplicate / blank title");
            }
        }
    }

    // Collection count properties
    public int RecipeCount => _recipeCatalogue.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _activeInstructions.Count;
    public int RemovedRecipeCount => _removedRecipeHistory.Count;

    // Private helper method: check whether a recipe ID exists in the cooking plan
    private bool IsRecipeInCookingPlan(int recipeId)
    {
        foreach (var id in _cookingPlan)
        {
            if (id == recipeId)
                return true;
        }

        return false;
    }

    // Part A: interface method implementations
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

        // Do not remove a recipe if it is currently in the cooking plan
        if (IsRecipeInCookingPlan(recipeId))
            return false;

        _recipeCatalogue.Remove(recipeId);
        return true;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        var recipe = FindRecipe(recipeId);

        if (recipe is null)
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
        LinkedListNode<int>? node = null;
        var current = _cookingPlan.First;

        while (current != null)
        {
            if (current.Value == recipeId)
            {
                node = current;
                break;
            }

            current = current.Next;
        }

        if (node is null)
            return false;

        _cookingPlan.Remove(node);

        // Store the removed recipe ID in the history stack
        _removedRecipeHistory.Push(recipeId);

        return true;
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

        if (_recipeCatalogue.ContainsKey(id) && !IsRecipeInCookingPlan(id))
        {
            _cookingPlan.AddLast(id);
            return true;
        }

        return false;
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        var list = new List<int>();

        foreach (var item in _cookingPlan)
        {
            list.Add(item);
        }

        return list.AsReadOnly();
    }

    public bool StartCooking(int recipeId)
    {
        var recipe = FindRecipe(recipeId);

        if (recipe is null)
            return false;

        if (recipe.Instructions.Count == 0)
            return false;

        // Clear previous instructions before starting a new recipe
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

    // Part B methods: keep these as NotImplementedException during Part A
   public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
    throw new NotImplementedException("Part B: implement SearchByTitle.");

public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
    throw new NotImplementedException("Part B: implement SearchByIngredient.");

public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
    throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

public bool AddSavedRecipe(int recipeId) =>
    throw new NotImplementedException("Part B: implement AddSavedRecipe.");

public bool RemoveSavedRecipe(int recipeId) =>
    throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

public bool IsRecipeSaved(int recipeId) =>
    throw new NotImplementedException("Part B: implement IsRecipeSaved.");

public IReadOnlyList<int> GetSavedRecipes() =>
    throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
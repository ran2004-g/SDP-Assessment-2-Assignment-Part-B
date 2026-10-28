1. Saved recipe collection
I selected HashSet<int> for the saved/favourite recipes feature. The set stores recipe IDs rather than duplicate Recipe objects, so the existing recipe catalogue (Dictionary<int, Recipe>) remains the source of recipe information. A hash set is suitable because saved recipe IDs must be unique and users need to add, remove and check saved recipes efficiently.
- Add: AddSavedRecipe(int recipeId) checks that the ID exists in _recipeCatalogue, then calls _savedRecipes.Add(recipeId). HashSet.Add returns false if the ID is already present, so duplicates are prevented automatically.
- Remove: RemoveSavedRecipe(int recipeId) calls _savedRecipes.Remove(recipeId) and returns whether an item was removed.
- Membership: IsRecipeSaved(int recipeId) uses _savedRecipes.Contains(recipeId).
- Display: GetSavedRecipes() copies the set to a list using ToList().AsReadOnly(), preventing callers from modifying the manager's internal collection through the returned value. A HashSet does not guarantee a particular display order.
These operations have expected average O(1) time for adding, removing and membership checks because the collection is hash-based. Producing a list of all s saved IDs takes O(s) time and additional space. Hash-set operations can be slower in unusual collision or resizing cases, so O(1) is an average expectation rather than an absolute guarantee.

I chose HashSet<int> instead of List<int> because a list would require an O(s) search to prevent duplicate IDs or check membership. I also did not use another dictionary, because the saved collection only needs to record membership, not store an additional value for each ID.
2. Integration with Part A
Part B extends the existing RecipeManager rather than creating a separate recipe catalogue. Part A's _recipeCatalogue (Dictionary<int, Recipe>) continues to own the recipes and support lookup by ID. The Part B title and ingredient searches apply LINQ to _recipeCatalogue.Values, using case-insensitive partial matching. If the search text is null, empty or whitespace, the current implementation returns all catalogue recipes. GetHighestProteinRecipes(int count) uses LINQ to exclude recipes without a protein value, order the remaining recipes by Nutrition.ProteinG in descending order and return up to count recipes. A non-positive count returns an empty list.

The new _savedRecipes field holds only IDs from the same catalogue, and AddSavedRecipe rejects IDs that do not exist at the time of saving. The Part A shopping list (List<string>), cooking plan (LinkedList<int>), removal history (Stack<int>) and active instructions (Queue<string>) are retained. The new functionality is exposed through the existing IRecipeManager API, allowing it to work alongside the Part A operations and application interface.

Current implementation limitation: RemoveRecipe does not remove that recipe's ID from _savedRecipes. Consequently, an ID that was saved earlier can remain in the saved set after its recipe is removed from the catalogue. This is a consistency edge case in the current code, not a behaviour that has been fixed by this design document.
3. Basic complexity
Let n be the number of recipes in the catalogue, p the number of IDs in the cooking plan, s the number of saved recipe IDs, k the number of recipes with protein data, and I the total number of ingredient entries examined during an ingredient search. Complexity estimates treat string lengths as bounded unless otherwise noted.
Operation	Structure / technique	Expected time complexity	Reason
Lookup recipe by ID (FindRecipe)	Dictionary<int, Recipe>	Average O(1)	Hash-based key lookup using TryGetValue.
Traverse cooking plan (GetCookingPlan)	LinkedList<int>	O(p)	Each planned recipe ID is visited and copied once.
Complete next instruction (CompleteNextInstruction)	Queue<string>	O(1)	Dequeue removes the front item without traversing the queue.
Search by title (SearchByTitle)	LINQ Where, Contains, ToList over dictionary values	O(n) for bounded title/search lengths	Each recipe title is checked; matches are collected into a list.
Search by ingredient (SearchByIngredient)	LINQ Where, Any, Contains, ToList	O(n + I) for bounded ingredient/search string lengths	Each recipe is visited and its ingredients may need to be checked; Any stops at the first match. If recipes have a bounded number of ingredients, this simplifies to O(n).
Highest-protein report (GetHighestProteinRecipes)	LINQ Where, OrderByDescending, Take	O(n + k log k), worst case O(n log n)	Scan for protein values, sort qualifying recipes, then take up to the requested count.
Add a saved recipe (AddSavedRecipe)	Dictionary membership check + HashSet<int>.Add	Average O(1)	Validate the ID using the dictionary, then use hash-set insertion; duplicates return false.
Remove a saved recipe (RemoveSavedRecipe)	HashSet<int>.Remove	Average O(1)	Hash-based removal by ID.
Check whether a recipe is saved (IsRecipeSaved)	HashSet<int>.Contains	Average O(1)	Hash-based membership test.
Get saved recipe IDs (GetSavedRecipes)	HashSet<int> + ToList	O(s)	Copies each saved ID into a read-only list view.


The searches and reports also allocate result lists proportional to the number of returned recipes. Sorting the protein report may require additional working memory. If title, ingredient or search-string lengths are allowed to grow without bound, string-comparison costs must also be included in the search complexity estimates.
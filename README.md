Recipe Management System
Project Introduction
This is a recipe management system I developed using C#, as part of Assessment 2 for COMP2021 Software Development Practice.  
The project consists of Part A and Part B. Part A implements basic recipe management functionality, while Part B adds search, protein ranking, and bookmarking features on top of the original implementation.

Part A — Core Functionality  
Part A primarily uses the following five C# collections:  
• Dictionary<int, Recipe>: Stores recipes.  
• List<string>: Maintains shopping lists.  
• LinkedList<int>: Manages cooking schedules.  
• Stack<int>: Records removed recipes from the cooking schedule for easy recovery.  
• Queue<string>: Stores cooking steps in sequence.  
Users can add, search, and delete recipes, as well as manage shopping lists, cooking schedules, and cooking steps.

Part B — Enhanced Features  
Part B introduces the following new features:  
1. Search by name: Uses LINQ to find recipes matching specified criteria.  
2. Search by ingredient: Finds recipes containing specific ingredients.  
3. Protein ranking: Displays recipes sorted by protein content in descending order.  
4. Bookmark recipes: Uses HashSet<int> to store bookmarked recipe IDs, preventing duplicates.  
All these features have been integrated into the original RecipeManager class.

How to Run  
Open a terminal in the project directory.  
Compile the project:
dotnet build
Run test:
dotnet test
Run the program:
dotnet run --project RecipeManagement.Application -- data/recipes.json
Test
I wrote unit tests for Part B to verify that the search, protein ranking, and bookmarking features work correctly, and also tested some invalid input scenarios.
Design.md
The Design.md file explains why these collections were chosen and outlines the time complexity of the main operations.  
AI Usage Statement  
During the completion of this assignment, I acknowledge that I used ChatGPT as an AI assistant to help me understand the requirements and grading details for both parts A and B of the project. For areas where my knowledge was insufficient, I relied on the AI to assist in comprehension. Regarding syntax errors and runtime bugs encountered during coding, I used the AI to analyze, guide corrections, provide fixes, supplements, and conduct code testing.  
All fundamental code writing was completed independently by myself, using only basic C# concepts entirely derived from the knowledge learned in this course.
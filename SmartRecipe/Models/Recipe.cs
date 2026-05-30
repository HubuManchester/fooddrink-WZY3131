namespace SmartRecipe.Models;

/// <summary>
/// Represents a recipe entity with all its properties and ingredients.
/// Follows the MVVM pattern as the core data model.
/// </summary>
public class Recipe
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public int CookingTimeMinutes { get; set; }
    public string Difficulty { get; set; } = "Easy";
    public List<string> Ingredients { get; set; } = new();
    public List<string> Instructions { get; set; } = new();
    public string CuisineType { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int Calories { get; set; }
    public double Rating { get; set; }
}
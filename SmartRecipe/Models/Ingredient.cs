namespace SmartRecipe.Models;

/// <summary>
/// Represents a scanned or searched ingredient.
/// Used for camera-based ingredient recognition and recipe matching.
/// </summary>
public class Ingredient
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime ScannedAt { get; set; } = DateTime.Now;
}
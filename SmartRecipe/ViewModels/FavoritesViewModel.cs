using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SmartRecipe.Models;
using SmartRecipe.Services;

namespace SmartRecipe.ViewModels;

/// <summary>
/// ViewModel for managing and displaying favorite recipes.
/// </summary>
public class FavoritesViewModel : INotifyPropertyChanged
{
    private readonly FavoriteService _favoriteService;
    private List<Recipe> _allRecipes;

    public ObservableCollection<Recipe> FavoriteRecipes { get; } = new();

    public FavoritesViewModel()
    {
        _favoriteService = new FavoriteService();
        _allRecipes = GetAllRecipes();
    }

    /// <summary>
    /// Refreshes the favorites list from persistent storage.
    /// </summary>
    public void RefreshFavorites()
    {
        try
        {
            FavoriteRecipes.Clear();
            var favoriteIds = _favoriteService.GetFavoriteIds();
            var favorites = _allRecipes.Where(r => favoriteIds.Contains(r.Id));

            foreach (var recipe in favorites)
            {
                FavoriteRecipes.Add(recipe);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading favorites: {ex.Message}");
        }
    }

    private List<Recipe> GetAllRecipes()
    {
        // Return consolidated sample recipes
        return new List<Recipe>
        {
            new()
            {
                Id = "1",
                Name = "Classic Margherita Pizza",
                CuisineType = "Italian",
                CookingTimeMinutes = 25,
                ImageUrl = "pizza_margherita.jpg"
            },
            new()
            {
                Id = "2",
                Name = "Thai Green Curry",
                CuisineType = "Thai",
                CookingTimeMinutes = 30,
                ImageUrl = "thai_green_curry.jpg"
            }
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
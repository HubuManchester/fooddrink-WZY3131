namespace SmartRecipe.Services;

/// <summary>
/// Manages user's favorite recipes using persistent storage.
/// Implements proper validation and error handling for data operations.
/// </summary>
public class FavoriteService
{
    private const string FavoritesKey = "favorite_recipes";
    private HashSet<string> _favoriteIds = new();

    public FavoriteService()
    {
        LoadFavorites();
    }

    /// <summary>
    /// Loads favorites from app preferences with validation.
    /// </summary>
    private void LoadFavorites()
    {
        try
        {
            var saved = Preferences.Get(FavoritesKey, string.Empty);
            if (!string.IsNullOrEmpty(saved))
            {
                _favoriteIds = new HashSet<string>(saved.Split(',',
                    StringSplitOptions.RemoveEmptyEntries));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading favorites: {ex.Message}");
            _favoriteIds = new HashSet<string>();
        }
    }

    /// <summary>
    /// Validates and saves favorites to persistent storage.
    /// </summary>
    private void SaveFavorites()
    {
        try
        {
            var data = string.Join(",", _favoriteIds);
            Preferences.Set(FavoritesKey, data);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving favorites: {ex.Message}");
            throw new InvalidOperationException("Failed to save favorites. Please try again.", ex);
        }
    }

    public bool IsFavorite(string recipeId)
    {
        return _favoriteIds.Contains(recipeId);
    }

    public void ToggleFavorite(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
        {
            throw new ArgumentException("Recipe ID cannot be empty.", nameof(recipeId));
        }

        if (_favoriteIds.Contains(recipeId))
        {
            _favoriteIds.Remove(recipeId);
        }
        else
        {
            _favoriteIds.Add(recipeId);
        }

        SaveFavorites();
    }

    public List<string> GetFavoriteIds()
    {
        return _favoriteIds.ToList();
    }
}
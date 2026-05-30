using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SmartRecipe.Models;

namespace SmartRecipe.ViewModels;

public class RecipesViewModel : INotifyPropertyChanged
{
    private List<Recipe> _allRecipes = new();
    private string _searchQuery = string.Empty;
    private int _currentPage = 1;
    private const int PageSize = 10;

    public ObservableCollection<Recipe> FilteredRecipes { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (_searchQuery != value)
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterRecipes();
            }
        }
    }

    public ICommand SearchCommand { get; }

    public RecipesViewModel()
    {
        SearchCommand = new Command(FilterRecipes);
        LoadSampleRecipes();
        FilterRecipes();
    }

    private void LoadSampleRecipes()
    {
        _allRecipes = new List<Recipe>
        {
            new()
            {
                Id = "1",
                Name = "Classic Margherita Pizza",
                Description = "Traditional Italian pizza with fresh mozzarella, tomatoes, and basil",
                ImageUrl = "pizza_margherita.jpg",
                CookingTimeMinutes = 25,
                Difficulty = "Easy",
                CuisineType = "Italian",
                Region = "Campania",
                Rating = 4.8,
                Calories = 250,
                Ingredients = new List<string> { "Pizza dough", "San Marzano tomatoes",
                                                "Fresh mozzarella", "Fresh basil", "Olive oil" },
                Instructions = new List<string>
                {
                    "Preheat oven to 250°C (480°F)",
                    "Stretch the pizza dough into a round shape",
                    "Spread crushed tomatoes on the dough",
                    "Add torn mozzarella pieces",
                    "Bake for 10-12 minutes until golden",
                    "Top with fresh basil leaves and drizzle with olive oil"
                }
            },
            new()
            {
                Id = "2",
                Name = "Thai Green Curry",
                Description = "Aromatic and creamy Thai curry with vegetables and coconut milk",
                ImageUrl = "thai_green_curry.jpg",
                CookingTimeMinutes = 30,
                Difficulty = "Medium",
                CuisineType = "Thai",
                Region = "Central Thailand",
                Rating = 4.6,
                Calories = 350,
                Ingredients = new List<string> { "Coconut milk", "Green curry paste",
                                                "Chicken breast", "Thai basil", "Bamboo shoots" },
                Instructions = new List<string>
                {
                    "Heat oil in a wok over medium-high heat",
                    "Add green curry paste and stir-fry for 1 minute",
                    "Pour in coconut milk and bring to a simmer",
                    "Add sliced chicken and cook for 10 minutes",
                    "Add vegetables and cook for 5 more minutes",
                    "Season with fish sauce and sugar",
                    "Garnish with Thai basil and serve with rice"
                }
            }
        };
    }

    private void FilterRecipes()
    {
        IEnumerable<Recipe> filtered = _allRecipes;

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.ToLowerInvariant();
            filtered = _allRecipes.Where(r =>
                r.Name.ToLowerInvariant().Contains(query) ||
                r.Ingredients.Any(i => i.ToLowerInvariant().Contains(query)) ||
                r.CuisineType.ToLowerInvariant().Contains(query)
            );
        }

        FilteredRecipes.Clear();
        var pagedResults = filtered.Take(PageSize);
        foreach (var recipe in pagedResults)
        {
            FilteredRecipes.Add(recipe);
        }
    }

    public void LoadMoreRecipes()
    {
        _currentPage++;
        var nextPage = _allRecipes.Skip(_currentPage * PageSize).Take(PageSize);
        foreach (var recipe in nextPage)
        {
            if (!FilteredRecipes.Contains(recipe))
            {
                FilteredRecipes.Add(recipe);
            }
        }
    }

    public void RefreshRecipes()
    {
        _currentPage = 1;
        FilterRecipes();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
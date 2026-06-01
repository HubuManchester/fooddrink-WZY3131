using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SmartRecipe.Models;
using SmartRecipe.Helpers;

namespace SmartRecipe.ViewModels
{
    public class RecipesViewModel : INotifyPropertyChanged
    {
        private List<Recipe> _allRecipes = new();
        private string _searchQuery = string.Empty;
        private int _currentPage = 1;
        private const int PageSize = 10;
        private bool _isDarkMode;

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
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DarkModeIcon));

                    // 切换主题
                    ThemeHelper.SetTheme(value
                        ? ThemeHelper.AppTheme.Dark
                        : ThemeHelper.AppTheme.Light);
                }
            }
        }
        public string DarkModeIcon => _isDarkMode ? "☀️" : "🌙";
        public ICommand SearchCommand { get; }

        public RecipesViewModel()
        {
            _isDarkMode = Application.Current?.RequestedTheme == Microsoft.Maui.ApplicationModel.AppTheme.Dark;
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
                    Ingredients = new List<string> { "Pizza dough", "San Marzano tomatoes", "Fresh mozzarella", "Fresh basil", "Olive oil" },
                    Instructions = new List<string>
                    {
                        "Preheat oven to 250°C (480°F)",
                        "Stretch the pizza dough into a round shape",
                        "Spread crushed tomatoes on the dough",
                        "Add torn mozzarella pieces",
                        "Bake for 10-12 minutes until golden",
                        "Top with fresh basil leaves and drizzle with olive oil"
                    }
                }
            };
        }

        private void FilterRecipes()
        {
            FilteredRecipes.Clear();
            var filtered = _allRecipes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLowerInvariant();
                filtered = _allRecipes.Where(r =>
                    r.Name.ToLowerInvariant().Contains(query) ||
                    r.Ingredients.Any(i => i.ToLowerInvariant().Contains(query)) ||
                    r.CuisineType.ToLowerInvariant().Contains(query));
            }

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
}
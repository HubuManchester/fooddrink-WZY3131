using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SmartRecipe.Models;
using SmartRecipe.Services;

namespace SmartRecipe.ViewModels;

/// <summary>
/// ViewModel for location-based cuisine discovery.
/// Implements hardware features: Location/GPS and Geocoding.
/// </summary>
public class LocalCuisineViewModel : INotifyPropertyChanged
{
    private readonly LocationService _locationService;
    private string _currentLocation = "Detecting location...";
    private string _locationSubtitle = string.Empty;
    private bool _isLoading;
    private bool _hasError;
    private string _errorMessage = string.Empty;

    public ObservableCollection<Recipe> LocalRecipes { get; } = new();

    public string CurrentLocation
    {
        get => _currentLocation;
        set { _currentLocation = value; OnPropertyChanged(); }
    }

    public string LocationSubtitle
    {
        get => _locationSubtitle;
        set { _locationSubtitle = value; OnPropertyChanged(); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotLoading));
        }
    }

    public bool IsNotLoading => !IsLoading;
    public bool HasRecipes => LocalRecipes.Count > 0;

    public bool HasError
    {
        get => _hasError;
        set { _hasError = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand RefreshLocationCommand { get; }

    public LocalCuisineViewModel()
    {
        _locationService = new LocationService();
        RefreshLocationCommand = new Command(async () => await RefreshLocation());
    }

    /// <summary>
    /// Gets current device location and loads regional recipes.
    /// Demonstrates GPS hardware usage with proper error handling.
    /// </summary>
    public async Task RefreshLocation()
    {
        try
        {
            IsLoading = true;
            HasError = false;

            var location = await _locationService.GetCurrentLocationAsync();
            if (location == null)
            {
                ShowError("Unable to detect your location. Please check your GPS settings.");
                return;
            }

            var placemarks = await _locationService.GetPlacemarksAsync(location);
            var placemark = placemarks.FirstOrDefault();

            if (placemark != null)
            {
                CurrentLocation = placemark.Locality ?? placemark.AdminArea ?? "Unknown Area";
                LocationSubtitle = $"{placemark.CountryName} - Lat: {location.Latitude:F2}, Lon: {location.Longitude:F2}";

                // Load recipes based on location
                LoadRecipesByRegion(placemark.CountryName ?? "Unknown");
            }
            else
            {
                CurrentLocation = $"Lat: {location.Latitude:F2}, Lon: {location.Longitude:F2}";
                LocationSubtitle = "Location detected (reverse geocoding unavailable)";
                LoadRecipesByRegion("Unknown");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(HasRecipes));
        }
    }

    /// <summary>
    /// Maps detected location to relevant regional recipes.
    /// </summary>
    private void LoadRecipesByRegion(string country)
    {
        LocalRecipes.Clear();

        // Map countries to cuisine types
        var regionRecipeMap = new Dictionary<string, List<string>>
        {
            ["Italy"] = new() { "Italian" },
            ["Japan"] = new() { "Japanese" },
            ["Thailand"] = new() { "Thai" },
            ["Mexico"] = new() { "Mexican" },
            ["Spain"] = new() { "Spanish" },
            ["France"] = new() { "French" },
            ["India"] = new() { "Indian" },
            ["China"] = new() { "Chinese" }
        };

        var cuisineTypes = regionRecipeMap.GetValueOrDefault(country, new List<string> { "General" });

        // Sample local recipes based on cuisine type
        var allRecipes = GetSampleRecipes();
        var matchingRecipes = allRecipes.Where(r => cuisineTypes.Contains(r.CuisineType));

        foreach (var recipe in matchingRecipes)
        {
            LocalRecipes.Add(recipe);
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private List<Recipe> GetSampleRecipes()
    {
        // Return same sample data as other ViewModels
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
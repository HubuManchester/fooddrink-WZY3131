using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SmartRecipe.Models;
using SmartRecipe.Services;

namespace SmartRecipe.ViewModels;

/// <summary>
/// ViewModel for the ingredient scanning page.
/// Manages camera capture and ingredient detection using device camera hardware.
/// </summary>
public class ScanViewModel : INotifyPropertyChanged
{
    private readonly CameraService _cameraService;
    private bool _isScanning;
    private bool _hasError;
    private string _errorMessage = string.Empty;
    private string _capturedImagePath = string.Empty;

    public ObservableCollection<Ingredient> DetectedIngredients { get; } = new();

    public bool IsScanning
    {
        get => _isScanning;
        set
        {
            _isScanning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotScanning));
        }
    }

    public bool IsNotScanning => !IsScanning;

    public bool HasDetectedIngredients => DetectedIngredients.Count > 0;
    public bool ShowEmptyState => !IsScanning && DetectedIngredients.Count == 0;
    public string CapturedImagePath
    {
        get => _capturedImagePath;
        set { _capturedImagePath = value; OnPropertyChanged(); }
    }

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

    public ICommand CaptureCommand { get; }
    public ICommand FindRecipesCommand { get; }
    public ICommand DismissErrorCommand { get; }

    public ScanViewModel()
    {
        _cameraService = new CameraService();
        CaptureCommand = new Command(async () => await CaptureIngredients());
        FindRecipesCommand = new Command(async () => await FindMatchingRecipes());
        DismissErrorCommand = new Command(DismissError);
    }

    public void Initialize()
    {
        UpdateStates();
    }

    /// <summary>
    /// Captures photo using device camera and processes for ingredients.
    /// Implements hardware feature: Camera with error handling.
    /// </summary>
    private async Task CaptureIngredients()
    {
        try
        {
            IsScanning = true;
            HasError = false;

            // Check and request camera permission
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    ShowError("Camera permission is required to scan ingredients. Please enable it in your device settings.");
                    return;
                }
            }

            var ingredients = await _cameraService.CaptureAndAnalyzeAsync();

            if (ingredients.Any())
            {
                DetectedIngredients.Clear();
                foreach (var ingredient in ingredients)
                {
                    DetectedIngredients.Add(ingredient);
                }
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }
            else
            {
                ShowError("No ingredients detected. Please try again with better lighting.");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsScanning = false;
            UpdateStates();
        }
    }

    /// <summary>
    /// Navigates to recipes filtered by detected ingredients.
    /// </summary>
    private async Task FindMatchingRecipes()
    {
        if (!DetectedIngredients.Any()) return;

        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        var ingredientNames = string.Join(",", DetectedIngredients.Select(i => i.Name));
        await Shell.Current.GoToAsync($"//Recipes?ingredients={ingredientNames}");
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void DismissError()
    {
        HasError = false;
        ErrorMessage = string.Empty;
    }

    private void UpdateStates()
    {
        OnPropertyChanged(nameof(HasDetectedIngredients));
        OnPropertyChanged(nameof(ShowEmptyState));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
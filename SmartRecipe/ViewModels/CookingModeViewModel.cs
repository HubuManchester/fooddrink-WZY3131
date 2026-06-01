using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SmartRecipe.Models;
using SmartRecipe.Services;

namespace SmartRecipe.ViewModels;

/// <summary>
/// ViewModel for the cooking mode step-by-step navigation.
/// Implements haptic feedback and timer functionality.
/// </summary>
public class CookingModeViewModel : INotifyPropertyChanged
{
    private readonly TextToSpeechService _ttsService;
    private List<string> _instructions = new();
    private int _currentStepIndex;
    private bool _isTimerRunning;
    private int _timerSeconds;
    private IDispatcherTimer? _timer;

    public string CurrentInstruction { get; set; } = string.Empty;
    public int CurrentStepNumber => _currentStepIndex + 1;
    public int TotalSteps => _instructions.Count;
    public bool CanGoPrevious => _currentStepIndex > 0;
    public bool CanGoNext => _currentStepIndex < _instructions.Count - 1;
    public bool IsTimerRunning
    {
        get => _isTimerRunning;
        set { _isTimerRunning = value; OnPropertyChanged(); }
    }
    public string TimerDisplay
    {
        get
        {
            var minutes = _timerSeconds / 60;
            var seconds = _timerSeconds % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }

    public ObservableCollection<bool> StepIndicators { get; } = new();

    public ICommand NextStepCommand { get; }
    public ICommand PreviousStepCommand { get; }
    public ICommand ReadStepCommand { get; }
    public ICommand StartTimerCommand { get; }

    public CookingModeViewModel()
    {
        _ttsService = new TextToSpeechService();
        NextStepCommand = new Command(GoToNextStep);
        PreviousStepCommand = new Command(GoToPreviousStep);
        ReadStepCommand = new Command(async () => await ReadCurrentStep());
        StartTimerCommand = new Command(ToggleTimer);
    }

    /// <summary>
    /// Loads recipe instructions for cooking mode.
    /// </summary>
    public void LoadRecipe(string recipeId)
    {
        var recipes = GetSampleRecipes();
        var recipe = recipes.FirstOrDefault(r => r.Id == recipeId);

        if (recipe != null)
        {
            _instructions = recipe.Instructions;
            InitializeSteps();
            UpdateCurrentStep();
        }
    }

    private void InitializeSteps()
    {
        StepIndicators.Clear();
        for (int i = 0; i < _instructions.Count; i++)
        {
            StepIndicators.Add(false);
        }
    }

    private void GoToNextStep()
    {
        if (_currentStepIndex < _instructions.Count - 1)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            StepIndicators[_currentStepIndex] = false;
            _currentStepIndex++;
            StepIndicators[_currentStepIndex] = true;
            UpdateCurrentStep();
        }
    }

    private void GoToPreviousStep()
    {
        if (_currentStepIndex > 0)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            StepIndicators[_currentStepIndex] = false;
            _currentStepIndex--;
            StepIndicators[_currentStepIndex] = true;
            UpdateCurrentStep();
        }
    }

    private async Task ReadCurrentStep()
    {
        var text = $"Step {CurrentStepNumber} of {TotalSteps}. {CurrentInstruction}";
        await _ttsService.SpeakAsync(text);
    }

    private void ToggleTimer()
    {
        if (_isTimerRunning)
        {
            StopTimer();
        }
        else
        {
            StartTimer();
        }
    }

    private void StartTimer()
    {
        _timerSeconds = 300; // 5-minute default timer
        IsTimerRunning = true;

        _timer = Application.Current?.Dispatcher.CreateTimer();
        if (_timer == null) return;

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) =>
        {
            if (_timerSeconds > 0)
            {
                _timerSeconds--;
                OnPropertyChanged(nameof(TimerDisplay));
                if (_timerSeconds == 0)
                {
                    HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                    StopTimer();
                }
            }
        };
        _timer.Start();
        OnPropertyChanged(nameof(TimerDisplay));
    }

    private void StopTimer()
    {
        _timer?.Stop();
        IsTimerRunning = false;
    }

    private void UpdateCurrentStep()
    {
        if (_currentStepIndex < _instructions.Count)
        {
            CurrentInstruction = _instructions[_currentStepIndex];
            OnPropertyChanged(nameof(CurrentInstruction));
            OnPropertyChanged(nameof(CurrentStepNumber));
            OnPropertyChanged(nameof(TotalSteps));
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }
    }

    private List<Recipe> GetSampleRecipes()
    {
        // Same sample data
        return new List<Recipe>
        {
            new()
            {
                Id = "1",
                Name = "Classic Margherita Pizza",
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
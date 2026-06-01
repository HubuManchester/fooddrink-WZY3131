using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SmartRecipe.Models;
using SmartRecipe.Services;

namespace SmartRecipe.ViewModels
{
    public class RecipeDetailViewModel : INotifyPropertyChanged
    {
        private Recipe? _recipe;
        private readonly TextToSpeechService _ttsService;

        public Recipe? Recipe
        {
            get => _recipe;
            set { _recipe = value; OnPropertyChanged(); }
        }

        public ICommand ReadAloudCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand StartCookingCommand { get; }

        public RecipeDetailViewModel()
        {
            _ttsService = new TextToSpeechService();
            ReadAloudCommand = new Command(async () => await ReadRecipeAloud());
            ToggleFavoriteCommand = new Command(ToggleFavorite);
            StartCookingCommand = new Command(async () => await StartCookingMode());
        }

        public void LoadRecipe(string recipeId)
        {
            var allRecipes = GetSampleRecipes();
            Recipe = allRecipes.FirstOrDefault(r => r.Id == recipeId);
        }

        private async Task ReadRecipeAloud()
        {
            if (Recipe == null) return;
            try
            {
                var text = $"Recipe: {Recipe.Name}. {Recipe.Description}.";
                await _ttsService.SpeakAsync(text);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        
        public void StopReading()
        {
            _ttsService.Stop();
        }

        private void ToggleFavorite()
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }

        private async Task StartCookingMode()
        {
            if (Recipe == null) return;
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync($"{nameof(Views.CookingModePage)}?recipeId={Recipe.Id}");
        }

        private List<Recipe> GetSampleRecipes()
        {
            return new List<Recipe>
            {
                new() { Id = "1", Name = "Classic Margherita Pizza" }
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
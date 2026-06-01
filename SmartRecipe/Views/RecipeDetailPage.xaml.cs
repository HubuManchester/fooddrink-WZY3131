using SmartRecipe.Models;
using SmartRecipe.Services;
using SmartRecipe.ViewModels;

namespace SmartRecipe.Views
{
    [QueryProperty(nameof(RecipeId), "recipeId")]
    public partial class RecipeDetailPage : ContentPage
    {
        private readonly RecipeDetailViewModel _viewModel;

        public string RecipeId
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _viewModel.LoadRecipe(value);
                }
            }
        }

        public RecipeDetailPage()
        {
            InitializeComponent();
            _viewModel = new RecipeDetailViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.StopReading();
        }
    }
}
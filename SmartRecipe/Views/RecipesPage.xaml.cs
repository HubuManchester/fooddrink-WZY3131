using SmartRecipe.Models;
using SmartRecipe.ViewModels;

namespace SmartRecipe.Views
{
    public partial class RecipesPage : ContentPage
    {
        private readonly RecipesViewModel _viewModel;

        public RecipesPage()
        {
            //InitializeComponent();
            _viewModel = new RecipesViewModel();
            BindingContext = _viewModel;
        }

        private async void OnRecipeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Recipe selectedRecipe)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                ((CollectionView)sender).SelectedItem = null;

                
                await Shell.Current.GoToAsync($"{nameof(RecipeDetailPage)}?recipeId={selectedRecipe.Id}");
            }
        }

        private async void OnRecipeCardTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Recipe recipe)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);

                
                await Shell.Current.GoToAsync($"{nameof(RecipeDetailPage)}?recipeId={recipe.Id}");
            }
        }

        private void OnLoadMoreItems(object sender, EventArgs e)
        {
            _viewModel.LoadMoreRecipes();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.RefreshRecipes();
        }
    }
}
using SmartRecipe.ViewModels;

namespace SmartRecipe.Views
{
    public partial class LocalCuisinePage : ContentPage
    {
        public LocalCuisinePage()
        {
            InitializeComponent();
        }

        
        private async void OnRecipeSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Models.Recipe selectedRecipe)
            {
                ((CollectionView)sender).SelectedItem = null;
                await DisplayAlert("Recipe Selected", $"You selected: {selectedRecipe.Name}", "OK");
            }
        }
    }
}
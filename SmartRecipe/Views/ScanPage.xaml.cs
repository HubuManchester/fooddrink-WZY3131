using SmartRecipe.ViewModels;

namespace SmartRecipe.Views;

public partial class ScanPage : ContentPage
{
    private readonly ScanViewModel _viewModel;

    public ScanPage()
    {
        InitializeComponent();
        _viewModel = new ScanViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize();
    }
}
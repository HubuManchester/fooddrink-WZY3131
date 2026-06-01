namespace SmartRecipe;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        
        ApplyFontScaling();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    
    private void ApplyFontScaling()
    {
        
        Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Unspecified;
    }
}
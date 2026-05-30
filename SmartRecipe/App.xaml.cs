namespace SmartRecipe;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // 必须是 AppShell，不是 MainPage
        return new Window(new AppShell());
    }
}
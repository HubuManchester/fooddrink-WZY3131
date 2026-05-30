using SmartRecipe.Views;  // 添加这个 using 语句

namespace SmartRecipe;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for detail navigation
        Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
        Routing.RegisterRoute(nameof(CookingModePage), typeof(CookingModePage));
    }
}
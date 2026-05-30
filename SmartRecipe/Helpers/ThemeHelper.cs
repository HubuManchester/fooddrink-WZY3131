namespace SmartRecipe.Helpers;

/// <summary>
/// Provides theme management functionality including dark mode toggle.
/// Supports accessibility requirement for high contrast modes.
/// </summary>
public static class ThemeHelper
{
    private const string ThemePreferenceKey = "SelectedTheme";

    public enum AppTheme
    {
        System,
        Light,
        Dark
    }

    /// <summary>
    /// Gets the currently saved theme preference from app storage.
    /// </summary>
    public static AppTheme GetSavedTheme()
    {
        var themeString = Preferences.Get(ThemePreferenceKey, AppTheme.System.ToString());
        return Enum.TryParse<AppTheme>(themeString, out var theme) ? theme : AppTheme.System;
    }

    /// <summary>
    /// Saves and applies the selected theme.
    /// </summary>
    public static void SetTheme(AppTheme theme)
    {
        Preferences.Set(ThemePreferenceKey, theme.ToString());
        ApplyTheme(theme);
    }

    /// <summary>
    /// Applies the theme to the current application instance.
    /// </summary>
    public static void ApplyTheme(AppTheme theme)
    {
        if (Application.Current == null) return;

        Application.Current.UserAppTheme = theme switch
        {
            AppTheme.Dark => Microsoft.Maui.ApplicationModel.AppTheme.Dark,
            AppTheme.Light => Microsoft.Maui.ApplicationModel.AppTheme.Light,
            _ => Microsoft.Maui.ApplicationModel.AppTheme.Unspecified
        };
    }

    /// <summary>
    /// Initializes the theme on app startup.
    /// </summary>
    public static void Initialize()
    {
        var savedTheme = GetSavedTheme();
        ApplyTheme(savedTheme);
    }
}
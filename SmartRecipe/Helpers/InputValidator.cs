namespace SmartRecipe.Helpers;

/// <summary>
/// Provides centralized input validation for all user inputs.
/// Follows WCAG accessibility guidelines for error messages.
/// </summary>
public static class InputValidator
{
    /// <summary>
    /// Validates search query input.
    /// Returns validation result with user-friendly error message.
    /// </summary>
    public static ValidationResult ValidateSearchQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return ValidationResult.Success(); // Empty search is valid, shows all results
        }

        if (query.Length > 100)
        {
            return ValidationResult.Failure("Search query must be 100 characters or fewer.");
        }

        if (query.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-'))
        {
            return ValidationResult.Failure("Search query contains invalid characters. Please use only letters, numbers, spaces, and hyphens.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates recipe name input for adding custom recipes.
    /// </summary>
    public static ValidationResult ValidateRecipeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationResult.Failure("Recipe name is required. Please enter a name for your recipe.");
        }

        if (name.Length < 3)
        {
            return ValidationResult.Failure("Recipe name must be at least 3 characters long.");
        }

        if (name.Length > 50)
        {
            return ValidationResult.Failure("Recipe name must be 50 characters or fewer.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates ingredient input format.
    /// </summary>
    public static ValidationResult ValidateIngredient(string ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient))
        {
            return ValidationResult.Failure("Ingredient name cannot be empty.");
        }

        if (ingredient.Length > 30)
        {
            return ValidationResult.Failure("Ingredient name must be 30 characters or fewer.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Validates email format for user profile.
    /// </summary>
    public static ValidationResult ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ValidationResult.Failure("Email address is required.");
        }

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            if (addr.Address != email)
            {
                return ValidationResult.Failure("Please enter a valid email address.");
            }
        }
        catch
        {
            return ValidationResult.Failure("Please enter a valid email address format.");
        }

        return ValidationResult.Success();
    }
}

/// <summary>
/// Represents the result of an input validation operation.
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
}
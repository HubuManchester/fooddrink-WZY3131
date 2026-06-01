using System.Collections.ObjectModel;

namespace SmartRecipe.Services;

/// <summary>
/// Service for camera-based ingredient scanning and barcode detection.
/// Implements hardware feature: Camera.
/// </summary>
public class CameraService
{
    private readonly List<string> _knownIngredients = new()
    {
        "tomato", "chicken", "onion", "garlic", "pasta", "rice",
        "cheese", "egg", "potato", "carrot", "broccoli", "fish",
        "beef", "pork", "mushroom", "pepper", "spinach", "lettuce"
    };

    /// <summary>
    /// Captures photo using device camera and processes for ingredient detection.
    /// In production, this would use computer vision/ML model for classification.
    /// </summary>
    public async Task<List<Models.Ingredient>> CaptureAndAnalyzeAsync()
    {
        try
        {
            // Check if camera is available
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                throw new InvalidOperationException("Camera is not available on this device.");
            }

            // Capture photo
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo == null) return new List<Models.Ingredient>();

            // In production: Process image with ML model
            // For demo: Return mock detected ingredients
            var detectedIngredients = await SimulateIngredientDetection();

            // Save photo for reference
            var localPath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
            using var sourceStream = await photo.OpenReadAsync();
            using var localStream = File.OpenWrite(localPath);
            await sourceStream.CopyToAsync(localStream);

            return detectedIngredients;
        }
        catch (PermissionException)
        {
            throw new InvalidOperationException("Camera permission is required. Please grant camera access in your device settings.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Camera error: {ex.Message}");
            throw new InvalidOperationException($"Failed to capture image: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Simulates ingredient detection. 
    /// In production, replace with actual computer vision model.
    /// </summary>
    private Task<List<Models.Ingredient>> SimulateIngredientDetection()
    {
        var random = new Random();
        var detectedCount = random.Next(2, 5);
        var ingredients = new List<Models.Ingredient>();

        for (int i = 0; i < detectedCount; i++)
        {
            var ingredientName = _knownIngredients[random.Next(_knownIngredients.Count)];
            ingredients.Add(new Models.Ingredient
            {
                Name = ingredientName,
                Category = "Food",
                ScannedAt = DateTime.Now
            });
        }

        return Task.FromResult(ingredients.DistinctBy(i => i.Name).ToList());
    }
}
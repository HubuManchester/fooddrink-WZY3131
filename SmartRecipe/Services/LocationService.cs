namespace SmartRecipe.Services;

/// <summary>
/// Service for device geolocation and reverse geocoding.
/// Implements hardware feature: Location/GPS.
/// </summary>
public class LocationService
{
    /// <summary>
    /// Gets the current device location with error handling.
    /// </summary>
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    throw new InvalidOperationException("Location permission is required. Please enable it in your device settings.");
                }
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            return location;
        }
        catch (FeatureNotSupportedException)
        {
            throw new InvalidOperationException("Location services are not supported on this device.");
        }
        catch (FeatureNotEnabledException)
        {
            throw new InvalidOperationException("Location services are disabled. Please enable them in your device settings.");
        }
        catch (PermissionException)
        {
            throw new InvalidOperationException("Location permission was denied.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Location error: {ex.Message}");
            throw new InvalidOperationException($"Failed to get location: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Performs reverse geocoding to get placemarks from coordinates.
    /// </summary>
    public async Task<IEnumerable<Placemark>> GetPlacemarksAsync(Location location)
    {
        try
        {
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(
                location.Latitude, location.Longitude);

            return placemarks;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Geocoding error: {ex.Message}");
            throw new InvalidOperationException($"Failed to determine your location details: {ex.Message}", ex);
        }
    }
}
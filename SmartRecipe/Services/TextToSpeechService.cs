namespace SmartRecipe.Services;


public class TextToSpeechService
{
    private CancellationTokenSource? _cts;

    /// <summary>
    /// Speaks the given text using device's TTS engine.
    /// Implements error handling to prevent app crashes.
    /// </summary>
    public async Task SpeakAsync(string text)
    {
        try
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            var options = new SpeechOptions
            {
                Pitch = 1.0f,
                Volume = 1.0f
            };

            await TextToSpeech.Default.SpeakAsync(text, options, _cts.Token);
        }
        catch (TaskCanceledException)
        {
            // TTS was intentionally stopped
        }
        catch (Exception ex)
        {
            // Log error and show user-friendly message
            System.Diagnostics.Debug.WriteLine($"TTS Error: {ex.Message}");
            throw new InvalidOperationException("Unable to read recipe. Please check your device's audio settings.", ex);
        }
    }

    /// <summary>
    /// Stops ongoing TTS playback.
    /// </summary>
    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }
}
namespace ChatApp.Models;

public class CopilotStudioOptions
{
    public const string SectionName = "CopilotStudio";
    
    public string GameAWebChatUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Validates that the webchat URL is from a trusted Copilot Studio domain
    /// </summary>
    public bool IsValidUrl()
    {
        if (string.IsNullOrWhiteSpace(GameAWebChatUrl))
            return false;
            
        if (!Uri.TryCreate(GameAWebChatUrl, UriKind.Absolute, out var uri))
            return false;
            
        // Only allow HTTPS URLs from copilotstudio.microsoft.com domain
        return uri.Scheme == "https" && 
               uri.Host.EndsWith("copilotstudio.microsoft.com", StringComparison.OrdinalIgnoreCase);
    }
}

namespace ChatApp.Models;

public class ChatMessage
{
    public string Role { get; set; } = string.Empty; // "User" or "Agent"
    public string Text { get; set; } = string.Empty;
    public string? ImageDataUrl { get; set; }
    public DateTime Timestamp { get; set; }
    public string? AgentName { get; set; }
}

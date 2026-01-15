namespace ChatApp.Models;

public class ScenarioRoot
{
    public List<Scenario> Scenarios { get; set; } = new();
}

public class Scenario
{
    public string AgentName { get; set; } = string.Empty;
    public List<Kadai> Kadai { get; set; } = new();
}

public class Kadai
{
    public string KadaiId { get; set; } = string.Empty;
    public List<Turn> Turns { get; set; } = new();
}

public class Turn
{
    public string? UserExact { get; set; }
    public List<string>? Keywords { get; set; }
    public string AgentReply { get; set; } = string.Empty;
    public string? AgentReplyCollapsedTitle { get; set; }
    public string? AgentReplyCollapsed { get; set; }
    public List<CollapsedSection>? AgentReplyCollapsedSections { get; set; }
}

public class CollapsedSection
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

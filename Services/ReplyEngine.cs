using ChatApp.Models;

namespace ChatApp.Services;

public class ReplyEngine
{
    private const int MinDelayMs = 500;
    private const int MaxDelayMs = 900;
    
    private readonly ScenarioRepository _repository;
    private readonly Random _random = new();

    public ReplyEngine(ScenarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Turn?> GetReplyAsync(string agentName, string userMessage, bool hasImage)
    {
        // Add random delay for demo effect (500-900ms)
        var delay = _random.Next(MinDelayMs, MaxDelayMs);
        await Task.Delay(delay);

        var scenario = _repository.GetScenarioByAgentName(agentName);
        if (scenario == null)
        {
            return new Turn { AgentReply = "申し訳ございません。このエージェントのシナリオが見つかりません。" };
        }

        // Determine kadai based on priority rules
        string kadaiId;
        if (hasImage)
        {
            kadaiId = "kadai1"; // Rule 1: Image attached
        }
        else if (userMessage.Contains("瞬間移動"))
        {
            kadaiId = "kadai2"; // Rule 2: Contains "瞬間移動"
        }
        else if (userMessage.Contains("全選択") || userMessage.Contains("記録") || userMessage.Contains("フラグ"))
        {
            kadaiId = "kadai3"; // Rule 3: Contains "全選択", "記録", or "フラグ"
        }
        else
        {
            // Rule 4: Default guide message
            return new Turn { AgentReply = "このデモは課題1〜3の想定質問に対応しています。テンプレート質問を使ってください。" };
        }

        var kadai = scenario.Kadai.FirstOrDefault(k => k.KadaiId == kadaiId);
        if (kadai == null || kadai.Turns.Count == 0)
        {
            return new Turn { AgentReply = "申し訳ございません。該当する課題が見つかりません。" };
        }

        // Find matching turn
        Turn? matchedTurn = null;

        // First, try exact match with userExact
        matchedTurn = kadai.Turns.FirstOrDefault(t => 
            !string.IsNullOrEmpty(t.UserExact) && 
            t.UserExact.Equals(userMessage.Trim(), StringComparison.OrdinalIgnoreCase));

        // If no exact match, try keyword partial match
        if (matchedTurn == null)
        {
            matchedTurn = kadai.Turns.FirstOrDefault(t => 
                t.Keywords != null && 
                t.Keywords.Any(kw => userMessage.Contains(kw, StringComparison.OrdinalIgnoreCase)));
        }

        // If still no match, return the first turn to avoid breaking the demo
        if (matchedTurn == null)
        {
            matchedTurn = kadai.Turns.First();
        }

        return matchedTurn;
    }
}

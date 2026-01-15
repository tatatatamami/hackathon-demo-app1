using System.Text.Json;
using ChatApp.Models;

namespace ChatApp.Services;

public class ScenarioRepository
{
    private ScenarioRoot? _scenarioRoot;
    private readonly IWebHostEnvironment _env;

    public ScenarioRepository(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task LoadScenariosAsync()
    {
        var filePath = Path.Combine(_env.WebRootPath, "scripts", "scenarios.json");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Scenario file not found: {filePath}");
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            _scenarioRoot = JsonSerializer.Deserialize<ScenarioRoot>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (_scenarioRoot == null)
            {
                throw new InvalidOperationException("Failed to deserialize scenario file: result is null");
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid JSON format in scenario file: {ex.Message}", ex);
        }
    }

    public Scenario? GetScenarioByAgentName(string agentName)
    {
        return _scenarioRoot?.Scenarios.FirstOrDefault(s => 
            s.AgentName.Equals(agentName, StringComparison.OrdinalIgnoreCase));
    }
}

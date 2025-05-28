using System.Text;
using System.Text.Json;

namespace ScrumAssistant.Services
{
    public class OpenAiService
    {
        private readonly string _apiKey;

        public OpenAiService(IConfiguration config)
        {
            _apiKey = config.GetSection("OpenAI")["ApiKey"];
        }

        public async Task<string> AnalyzeTicket(string summary, string description)
        {
            var prompt = $@"
You are a senior scrum master helping with sprint planning.

Analyze the following task:
{summary} - {description}

1. Estimate the story point (Fibonacci: 1, 2, 3, 5, 8, 13). Use a fun emoji to represent the size (e.g., 🐣 for 1, 🐥 for 2, 🐔 for 3, 🦉 for 5, 🦅 for 8, 🐉 for 13).
2. Rate clarity: Well-defined, Ambiguous, or Vague. Use an emoji (✅ for Well-defined, 🤔 for Ambiguous, ❓ for Vague).
3. Only suggest improvements if the clarity is Ambiguous or Vague. If the clarity is Well-defined, write ""None"" as the suggestion.

Return format:
Story Point: <number> <emoji>
Clarity: <clarity> <emoji>
Suggestion: <None or suggestions only if clarity is not Well-defined>
";


            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new object[]
                {
                    new { role = "system", content = "You are an expert scrum master." },
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return result.Trim();
        }
    }
}
using RestSharp;
using Newtonsoft.Json.Linq;

namespace ScrumAssistant.Services
{
    public class JiraService
    {
        private readonly string _baseUrl, _email, _apiToken;

        public JiraService(IConfiguration config)
        {
            var jiraConfig = config.GetSection("Jira");
            _baseUrl = jiraConfig["BaseUrl"];
            _email = jiraConfig["Email"];
            _apiToken = jiraConfig["ApiToken"];
        }

        public async Task<List<JiraTicket>> GetTicketsAsync(string projectKey)
        {
            var client = new RestClient(_baseUrl);
            var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_email}:{_apiToken}"));
            var tickets = new List<JiraTicket>();
            int startAt = 0;
            int maxResults = 50; // Up to 1000 if you want
            int total = 0;

            do
            {
                var jql = $"project={projectKey} AND statusCategory != Done";
                var request = new RestRequest($"/rest/api/3/search?jql={Uri.EscapeDataString(jql)}&startAt={startAt}&maxResults={maxResults}", Method.Get);

                request.AddHeader("Authorization", $"Basic {auth}");
                request.AddHeader("Accept", "application/json");

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                    throw new Exception("Jira API error: " + response.Content);

                var json = JObject.Parse(response.Content);

                if (json["issues"] is not JArray issues || issues.Count == 0)
                    break;

                foreach (var issue in issues)
                {
                    var fields = issue["fields"];
                    var descriptionToken = fields["description"];
                    string description = "";

                    if (descriptionToken != null)
                    {
                        if (descriptionToken.Type == JTokenType.Object)
                        {
                            var content = descriptionToken["content"];
                            if (content != null && content.Type == JTokenType.Array && content.HasValues)
                            {
                                var paragraph = content[0];
                                if (paragraph != null && paragraph["content"] != null && paragraph["content"].Type == JTokenType.Array && paragraph["content"].HasValues)
                                {
                                    var textToken = paragraph["content"][0]["text"];
                                    if (textToken != null)
                                        description = textToken.ToString();
                                }
                            }
                        }
                        else if (descriptionToken.Type == JTokenType.String)
                        {
                            description = descriptionToken.ToString();
                        }
                    }

                    tickets.Add(new JiraTicket
                    {
                        Key = issue["key"].ToString(),
                        Summary = fields["summary"].ToString(),
                        Description = description
                    });
                }

                // Set total from API response
                total = (int)json["total"];
                startAt += maxResults;
            }
            while (startAt < total);

            return tickets;
        }

        public async Task<List<(string Key, string Name)>> GetProjectsAsync()
        {
            var client = new RestClient(_baseUrl);
            var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_email}:{_apiToken}"));
            var request = new RestRequest($"/rest/api/3/project/search", Method.Get);
            request.AddHeader("Authorization", $"Basic {auth}");
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                throw new Exception("Jira API error: " + response.Content);

            var json = JObject.Parse(response.Content);
            var projects = new List<(string Key, string Name)>();

            foreach (var project in json["values"])
            {
                string key = project["key"]?.ToString() ?? "";
                string name = project["name"]?.ToString() ?? "";
                projects.Add((key, name));
            }
            return projects.OrderBy(p => p.Name).ToList();
        }

        public async Task<bool> AddCommentToTicketAsync(string ticketKey, string comment)
        {
            try
            {
                var client = new RestClient(_baseUrl);
                var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{_email}:{_apiToken}"));
                var request = new RestRequest($"/rest/api/3/issue/{ticketKey}/comment", Method.Post);
                request.AddHeader("Authorization", $"Basic {auth}");
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");

                // If plain text doesn't work, try this ADF format:
                var payload = new
                {
                    body = new
                    {
                        type = "doc",
                        version = 1,
                        content = new[]
                        {
                            new {
                                type = "paragraph",
                                content = new[]
                                {
                                    new {
                                        type = "text",
                                        text = comment
                                    }
                                }
                            }
                        }
                    }
                };
                request.AddJsonBody(payload);


                var response = await client.ExecuteAsync(request);
                return response.IsSuccessful;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }

    public class JiraTicket
    {
        public string Key { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
    }
}
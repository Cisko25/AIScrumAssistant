using Microsoft.AspNetCore.Mvc;
using ScrumAssistant.Services;
using ScrumAssistant.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScrumAssistant.Controllers;

public class HomeController : Controller
{
    private readonly JiraService _jira;
    private readonly OpenAiService _openai;
    private const int PageSize = 20;

    public HomeController(JiraService jira, OpenAiService openai)
    {
        _jira = jira;
        _openai = openai;
    }

    public async Task<IActionResult> Index(string projectKey = "", int page = 1, string searchTerm = "")
    {
        // Get all projects for dropdown
        var projects = await _jira.GetProjectsAsync();
        ViewBag.Projects = projects;

        // Default to the first project if none selected
        string selectedProjectKey = projectKey;
        if (string.IsNullOrWhiteSpace(selectedProjectKey) && projects.Any())
            selectedProjectKey = projects[0].Key;

        // Only fetch tickets for selected project
        var tickets = await _jira.GetTicketsAsync(selectedProjectKey);

        // ... (same search and paging logic as before)

        // Search/Filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            tickets = tickets.Where(t =>
                (!string.IsNullOrEmpty(t.Summary) && t.Summary.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(t.Key) && t.Key.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        // Paging
        var totalTickets = tickets.Count;
        var totalPages = (int)Math.Ceiling(totalTickets / (double)PageSize);
        var pagedTickets = tickets.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        var vm = new JiraTicketListViewModel
        {
            Tickets = pagedTickets,
            CurrentPage = page,
            TotalPages = totalPages,
            SearchTerm = searchTerm
        };

        ViewBag.SelectedProjectKey = selectedProjectKey;
        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> Analyze(string key, string summary, string description)
    {
        var result = await _openai.AnalyzeTicket(summary, description);
        ViewBag.Result = result;
        ViewBag.Ticket = new JiraTicket { Key = key, Summary = summary, Description = description };
        return View("AnalyzeResult");
    }

    [HttpPost]
    public async Task<IActionResult> CommentToJira(string key, string summary, string description, string analysis)
    {
        // Post the comment
        bool result = await _jira.AddCommentToTicketAsync(key, analysis);

        // Optionally, show a message
        ViewBag.CommentResult = result ? "Comment posted to Jira successfully!" : "Failed to post comment to Jira.";
        ViewBag.Result = analysis;
        ViewBag.Ticket = new JiraTicket { Key = key, Summary = summary, Description = description };

        return View("AnalyzeResult");
    }

}

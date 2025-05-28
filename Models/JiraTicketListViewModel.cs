using ScrumAssistant.Services;
using System.Collections.Generic;

namespace ScrumAssistant.Models;
public class JiraTicketListViewModel
{
    public List<JiraTicket> Tickets { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; }
}
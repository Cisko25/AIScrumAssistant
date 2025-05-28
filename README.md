# 🤖 AI Scrum Assistant Web App

## What is this App?

This is an **AI-powered Scrum Assistant** built as an ASP.NET Core MVC web application. It’s designed to help teams make sprint planning and grooming easier.

- **Integrates with Jira** to fetch your project tasks.
- Uses **OpenAI** to analyze each ticket—giving instant estimates, clarity ratings, and improvement suggestions.

---

## What Can It Do?

### 🟢 Seamless Jira Integration
- Connects to Jira using your API key.
- Lets you pick the project you want to view (by Project Key and Project Name).

### 📋 Sprint Task Overview with AI
- Displays your team’s current (not done) tasks, with paging and search/filter.
- Clean, friendly UI: search by keyword, filter by project, and page through long lists easily.

### ⚡ One-Click AI Ticket Analysis
- Each ticket/task has an **Analyze** button.
- Instantly generates an AI-powered summary, including:
  - **Story Point Estimate** (with a fun emoji size!)
  - **Clarity Rating** (Well-defined, Ambiguous, or Vague—with emoji)
  - **Improvement Suggestions** (when the task isn’t well-defined)
- AI feedback uses friendly language and emojis, making technical analysis approachable and actionable.

### 📝 Jira Commenting
- After reviewing the AI analysis, you can post it as a comment directly back to the Jira ticket with one click—keeping your team and workflow in sync.

---

## Why Is This Awesome?

- **Saves Time:** No more manual back-and-forth for every ticket. Get estimates and feedback in seconds.
- **Boosts Clarity:** AI helps flag unclear tickets and suggests how to make them better before the sprint even starts.

---

## Technical Highlights

- **Built with:**
  - `ASP.NET Core`, open-source web development framework | .NET Core MVC C# — clean and maintainable codebase
  - `OpenAI ChatGPT API` — for natural language analysis
  - `Jira REST API` — robust project/task integration

- **Features:**
  - Project selector (Project Key & Name)
  - Real-time search/filter & paging

---

## How To Use It

1. Use your Jira credentials (API key and email).
2. Choose your Jira project from the dropdown.
3. Search, filter, and page through your team’s active tickets.
4. Click **Analyze** on any ticket to get instant AI feedback.
5. Post the AI results as a comment to Jira with a single click.

---

## Who’s This For?

- Scrum Masters & Agile teams
- Anyone who wants to make backlog refinement and sprint planning easier—with a little AI magic ✨


# HR Screening Exam Platform

> A full-stack online examination platform for HR technical screening and candidate evaluation.

![.NET](https://img.shields.io/badge/.NET-ASP.NET_Core-512BD4?logo=dotnet)
![Entity Framework](https://img.shields.io/badge/ORM-Entity_Framework_Core-purple)
![Next.js](https://img.shields.io/badge/Frontend-Next.js-black?logo=next.js)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?logo=microsoftsqlserver)
![TailwindCSS](https://img.shields.io/badge/Styling-TailwindCSS-06B6D4?logo=tailwindcss)
![GitLab](https://img.shields.io/badge/VCS-GitLab-FC6D26?logo=gitlab)

---

## Demo Link

🎥 [View Demo on Google Drive](https://drive.google.com) — Screen recording of the full application flow.

---

## Project Overview

**HR Screening Exam Platform** is a complete recruitment assessment system built to simplify and automate the technical screening process. The project was developed collaboratively by a team of 4 interns within a single month as part of an intensive web development internship program at **Enozom**, covering the full product lifecycle — from planning and UI/UX design to backend development, frontend integration, and delivery.

---

## Features

### Core Platform Capabilities

- **Role-Based Access Control:** User authentication with role-based access control
- **RESTful API Integration:** Full CRUD operations across all resources via a structured Minimal API layer with consistent request/response contracts
- **Real-Time Dashboard:** Real-time dashboard with data filtering and search
- **Dual-Layer Validation:** Input validation enforced on both the frontend and backend with structured error feedback
- **Responsive UI:** Responsive interface optimized for desktop and mobile viewports

### Authentication & Security

- Secure admin login with token-based session management
- Persistent authentication via access tokens; all protected routes enforce token validity
- Automatic session expiration with redirect to login on token invalidation
- Forgot password flow with email-based password reset
- Tokenized exam access links for candidates, with server-side validation of invalid or expired tokens
- Invalid or expired exam links prevent candidates from starting the exam and display an appropriate status message

### Admin Portal

#### Candidate Management

- Add candidates and view detailed candidate profiles
- Delete candidates with cascading exam assignment cleanup
- Track per-candidate exam status across four states: **Pending**, **In Progress**, **Expired**, and **Done**
- Real-time candidate search by name or email

#### Question Bank

- Create, edit and delete multiple-choice questions
- Support for image-based questions and image-based answer choices
- Organize questions under categorized topics (e.g., React, HTML, OOP, Database & IQ)
- Create, edit and delete topics; cascading delete removes all associated questions
- Correct answer indicators stored per question for automated evaluation

#### Exam Management

- Create exams with a custom title and duration
- Build exams by selecting questions from the question bank or authoring new questions inline during exam creation
- Dynamically add, remove and edit questions within an exam at any time
- Edit existing exams, including metadata and question set
- Assign exams to candidates with custom access periods
- Trigger automated email invitations with secure, tokenized exam links upon assignment

#### User Management *(Owner only)*

- Dedicated admin management panel restricted to the Owner role
- Add new admin accounts to the platform

### Candidate Experience

#### Pre-Exam Flow

- Candidates receive a personalized email invitation containing a secure, tokenized exam link
- Invalid or expired links are rejected with an appropriate error state
- Pre-exam landing page displays exam instructions, total question count, and the duration
- Countdown timer displayed when the exam window has not yet opened

#### Exam Session

- Confirmation modal presented before the exam session is initiated
- Free navigation between questions throughout the session
- Real-time answer persistence to the database on every selection, preventing data loss
- Countdown timer displayed throughout the session with a visual warning as time approaches expiry
- Automatic exam submission triggered upon timer expiration

#### Post-Exam Flow

- Submission confirmation modal displaying answered vs. total questions before final submission
- Dedicated completion page rendered upon successful submission
- HR contact email displayed for candidate support and post-exam feedback

---

## Tech Stack

### Backend

| Technology | Purpose |
|---|---|
| ASP.NET Core Minimal APIs | RESTful API layer |
| Entity Framework Core | ORM and database abstraction |
| SQL Server / SQL | Relational data storage |
| C# | Primary backend language |

### Frontend

| Technology | Purpose |
|---|---|
| Next.js (React) | Frontend framework and routing |
| TypeScript | Type-safe UI logic and component development |
| HTML5 | Semantic page structure |
| Tailwind CSS | Styling and responsive layout |
| Zod | Schema validation for forms and API data |

### Tooling & Design

| Tool | Purpose |
|---|---|
| GitLab | Version control and merge request workflow |
| Jira | Sprint planning and task management |
| Slack | Team communication |
| Figma AI | UI/UX design prototyping |
| Claude AI | AI-assisted development support |

---

## Architecture Overview

The backend follows a **layered clean architecture** pattern with clear boundaries between responsibilities:

| Layer | Responsibility |
|---|---|
| `Exam.Api` | Minimal API endpoints, request/response handling |
| `Exam.Service` | Business Logic |
| `Exam.Repo` | Data Access Layer |
| `Exam.Data` | Database Context & Migrations |
| `Exam.Models` | Domain Models and DTOs |

The frontend follows a **feature-based component architecture** using Next.js App Router, with reusable UI components and API service modules.

---

## Installation & Setup

### Prerequisites

Make sure the following are installed on your machine:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 24+](https://nodejs.org/) and npm
- [SQL Server](https://www.microsoft.com/en-us/sql-server) or SQL Server Express
- [Git](https://git-scm.com/)

### Clone the Repository

```bash
git clone https://github.com/SSamakk/Exam-Platform.git
cd exam-platform
```

### Environment Variables

**Frontend:** `frontend/.env.local`

```env
NEXT_PUBLIC_API_URL="http://localhost:5129"
```

### Running the Backend

```bash
# Navigate to the API project
cd Backend/Exam.Api

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the development server
dotnet run
```

The API will be available at: `https://localhost:5129`  
Scalar UI: `https://localhost:5129/scalar`

### Running the Frontend

```bash
# Navigate to the frontend directory
cd Frontend/exan.ui

# Install dependencies
npm install

# Start the development server
npm run dev
```

The frontend will be available at: `http://localhost:3000`

---

## Database & Migrations

This project uses **Entity Framework Core** with Code-First migrations.

```bash
# Add a new migration
dotnet ef migrations add InitialCreate --project ../Exam.Data --startup-project Exam.Api

# Apply migrations to the database
dotnet ef database update --project ../Exam.Data --startup-project Exam.Api

# Revert last migration
dotnet ef migrations remove
```

The database context is defined in `Backend/Exam.Data/ApiContext.cs`

---

## Running Tests

```bash
# Navigate to the tests directory

## Unit Tests
cd Backend/Exam.Tests

## Integration Tests
cd Backend/Exam.IntegrationTests

# Run tests
dotnet test
```

---

## Development Workflow

This project followed an **Agile development process** with the following practices:

- **Sprint Planning** — Work was organized into weekly sprints using Jira. Tasks were estimated, assigned, and tracked on a shared board.
- **Branching Strategy** — Each feature was developed on a dedicated branch. Branches were merged via GitLab Merge Requests after review.
- **Code Reviews** — All MRs required approval before merging into main.
- **Daily Standups** — Brief daily syncs on Slack to communicate progress and blockers.
- **Testing** — Features were manually tested against acceptance criteria before marking tasks as done.

---

## Future Improvements

Potential future enhancements for the platform include:

- Containerize the application using **Docker** for scalable deployment and environment consistency
- Integrate **AI-assisted exam generation** with configurable difficulty levels (Intern, Junior, Mid-Level, Senior)
- Implement **dynamic question randomization** to create unique exam experiences per candidate

---

## Contributors

| Name | Role |
|---|---|
| [Elsayed Abdelfattah](https://github.com/Sayed1210) | Software Engineer |
| [Shaymaa Samak](https://github.com/SSamakk) | Software Engineer |
| [Seif Tahtawy](SeifTahtawy) | Software Engineer |
| [Menna Elsamadisy](Mennaelsamadisy) | Software Engineer |

Developed under the mentorship and guidance of **[Eng. Ibrahim Aly](Ibrahim-Aly)**.

---

<p align="center">
  <strong>Enozom "The Project" Internship 2026</strong>
</p>

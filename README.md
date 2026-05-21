# HR Screening Online Exam Platform

<p align="center">
  A full-stack online examination platform for HR technical screening and candidate evaluation.
</p>

<p align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?style=for-the-badge&logo=dotnet)
![Next JS](https://img.shields.io/badge/Next.js-Frontend-black?style=for-the-badge&logo=next.js)
![React](https://img.shields.io/badge/React-18-blue?style=for-the-badge&logo=react)
![TailwindCSS](https://img.shields.io/badge/TailwindCSS-Styling-38B2AC?style=for-the-badge&logo=tailwind-css)
![MySQL](https://img.shields.io/badge/MySQL-Database-orange?style=for-the-badge&logo=mysql)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

</p>

---

# Overview

The **HR Screening Online Exam Platform** is a complete recruitment assessment system built to simplify and automate the technical screening process.

The platform allows HR teams and technical interviewers to:

- Build and manage technical exams
- Maintain a reusable question bank
- Assign exams to candidates
- Send secure invitation emails
- Monitor candidate completion status
- Conduct timed online assessments

Candidates receive secure, time-sensitive exam links and complete assessments in a clean, distraction-free environment with a live timer and question navigation.

---

# Demo & Screenshots

Google Drive Folder:  


```txt
https://drive.google.com/drive/folders/1ziMk2c6dJ1VY61nYRHYsqSJwdAGY-ujY
```

---

# Features

## Admin Portal

### Authentication & Security
- Secure admin login
- Forgot password functionality
- Reset password via email
<!-- - Protected routes and APIs-->

---

### Candidate Management
- Add candidates
- View candidate details
- Delete candidates
- Track exam status:
  - Pending
  - In Progress
  - Expired
  - Done

---

### Question Bank
- Create technical questions
- Edit existing questions
- Delete questions
- Categorize by topic

Examples:
- React
- HTML/CSS
- OOP
- Data Structures & Algorithms

Supports:
- Multiple-choice questions
- Correct answer indicators

---

### Dynamic Exam Builder

Administrators can:

- Create exams with custom duration
- Add questions from the question bank
- Create new questions directly inside exam creation
- Edit existing exams
- Add/remove/update questions dynamically

---

### Automated Email Invitations
- Assign exams to candidates
- Send exam links via email
- Set custom expiration dates
- Email-based exam access flow

---

## Candidate Experience

### Exam Landing Page

Before starting, candidates can view:

- Exam instructions
- Number of questions
- Total exam duration

---

### Active Exam Environment

Features include:

- Live countdown timer
- Distraction-free UI
- Question navigation panel
- Answer tracking
- Final submission handling

---

# Tech Stack

## Frontend
- Next.js
- React
- Tailwind CSS
- Heroicons
- Zod

---

## Backend
- ASP.NET Core Web API (.NET)

### Backend Architecture

The backend follows a layered architecture:

| Layer | Responsibility |
|------|------|
| `Exam.Api` | API & Controllers |
| `Exam.Service` | Business Logic |
| `Exam.Repo` | Data Access Layer |
| `Exam.Data` | Database Context & Config |
| `Exam.Models` | Domain Models |

---

## Testing Projects

- `Exam.Tests`
- `Exam.IntegrationTests`

---

# Getting Started

## Prerequisites

Before running the project, ensure you have installed:

- Node.js (v18+ recommended)
- .NET SDK
- MySQL Server
- Git

---

# Installation & Setup

## Clone the Repository

```bash
git clone https://github.com/yourusername/Exam-platform.git
cd Exam-Platform
```

---

# Frontend Setup

Navigate to the frontend directory:

```bash
cd Frontend/exam.ui
```

Install dependencies:

```bash
npm install
```

Setup environment variables:

```bash
cp .env.example .env.local
```

Run the development server:

```bash
npm run dev
```

Frontend runs on:

```txt
http://localhost:3000
```

---

# Backend Setup

Navigate to backend:

```bash
cd Backend/Exam.Api
```

Restore packages:

```bash
dotnet restore
```

Update your database connection string inside:

```txt
appsettings.json
```

Run migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

Backend API will run on:

```txt
https://localhost:5129
```

---

# Authentication Features

The system supports:

- Admin authentication
- Password reset flow
- Secure email invitations
<!--- Protected APIs-->
- Timed exam access

---

<!--# Exam Workflow

```txt
Admin Creates Exam
        ↓
Admin Assigns Candidate
        ↓
System Sends Email Invitation
        ↓
Candidate Opens Secure Link
        ↓
Candidate Takes Timed Exam
        ↓
Candidate Submits Assessment
        ↓
Admin Reviews Completion Status 
```-->

---

# Running Tests

Run all tests using:

```bash
dotnet test
```
---

# Maintainer

Maintained by:

**Your Name / GitHub Username**

---
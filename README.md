# Acme Global College (VGC) — Student & Course Management System

ASP.NET Core MVC web application for managing students, courses, and academic progress across three college branches.

## Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core + SQLite
- ASP.NET Core Identity (authentication + RBAC)
- xUnit (unit tests)
- GitHub Actions (CI)

## How to Run Locally

1. Clone the repository
```bash
git clone https://github.com/Oleg-Dergunov/oop-s2-3-mvc-76522
```
2. Open `oop-s2-3-mvc-76522.sln` in Visual Studio
3. Set `VgcCollege.Web` as the startup project
4. Run the application (F5)
5. The database is created and seeded automatically on first run

No manual migrations required — the app runs `Update-Database` equivalent on startup via `DataSeeder`.

## How to Run Without Visual Studio

1. Install .NET 8 SDK:
   - Windows: `winget install Microsoft.DotNet.SDK.8`
   - Mac: `brew install dotnet@8`
   - Linux: `sudo apt-get install -y dotnet-sdk-8.0`

2. Clone the repository:
```bash
git clone https://github.com/Oleg-Dergunov/oop-s2-3-mvc-76522
```
3. Navigate to the web project:
```bash
cd oop-s2-3-mvc-76522/VgcCollege.Web
```
4. Run the app:
```bash
dotnet run
```
5. Open browser at `https://localhost:5001` or `http://localhost:5000`

## How to Run Tests

In Visual Studio:
- Open Test Explorer (Test → Test Explorer)
- Click Run All

Or via terminal:
```bash
dotnet test
```

## Seeded Demo Accounts

| Role    | Email              | Password    |
|---------|--------------------|-------------|
| Admin   | admin@vgc.ie       | Admin@123   |
| Faculty | faculty1@vgc.ie    | Faculty@123 |
| Faculty | faculty2@vgc.ie    | Faculty@123 |
| Student | student1@vgc.ie    | Student@123 |
| Student | student2@vgc.ie    | Student@123 |
| Student | student3@vgc.ie    | Student@123 |

## Seeded Data

- 3 branches: Dublin, Cork, Galway
- 3 courses assigned to faculty across branches
- 4 enrolments with attendance records
- Assignments and exam results (one exam released, one provisional)

## Design Decisions

- **SQLite** used instead of SQL Server for simplicity and CI compatibility
- **VgcCollege.Domain** class library holds all entity models, keeping them separate from the web layer
- **DataSeeder** runs on startup and only seeds if the database is empty
- **DateOnly** used for all date fields (no time component needed)
- **AttendanceStatus enum** (Present/Absent/NA) instead of a boolean for more realistic tracking
- **GradeCalculator** helper automatically calculates grades from scores (A/B/C/D/E)
- **ResultsReleased flag** on Exam controls student visibility of exam results — enforced server-side
- Admin creates all user accounts (no public registration) to maintain controlled access
- Faculty can only view students enrolled in their own courses — enforced via server-side query filtering
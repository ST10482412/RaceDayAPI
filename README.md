# RaceDayAPI
RaceDayAPI web API project
# RaceDayAPI – Race Day Management System

## 1. Project Overview

RaceDayAPI is an ASP.NET Core Web API developed to support the management of race day events. The system is designed to organise race events, manage participants and organisers, handle event categories, record enrolments, and store race results.

The API uses a structured database to keep race information organised and to support communication between the application and its data.

## 2. Project Objectives

The main objectives of RaceDayAPI are to:

- Manage user registration and login.
- Support different user roles, including Admin, Organiser, and Participant.
- Manage race events and their details.
- Create and manage categories for events.
- Register participants for event categories.
- Record participant results, finishing positions, and finishing times.
- Store and manage information using a relational database.

## 3. Technologies Used

- **C#** – Main programming language.
- **ASP.NET Core Web API** – Used to develop the API and its endpoints.
- **Entity Framework Core** – Used for database access and object-relational mapping.
- **Microsoft SQL Server** – Database management system.
- **Visual Studio** – Development environment.
- **Swagger / OpenAPI** – Used to view and test API endpoints during development.
- **GitHub** – Used for source code management and project submission.

## 4. Main System Entities

The project contains the following main entities:

| Entity | Purpose |
|---|---|
| User | Stores user details, login information, and roles. |
| Organiser | Stores information about event organisers. |
| Participant | Stores information about race participants. |
| Event | Stores race event details, including name, location, and status. |
| Category | Stores the different categories available for an event. |
| Enrolment | Records participants registered for event categories. |
| Result | Stores race results, finishing positions, times, and status. |

These entities are connected through relationships and foreign keys to help maintain data consistency.

## 5. Database

RaceDayAPI uses Microsoft SQL Server as its database.

The database is configured through the application's connection string and Entity Framework Core's `RaceDayContext` class.

The context defines the database tables, primary keys, foreign keys, unique indexes, default values, and validation constraints.

The database is designed to help prevent duplicate email addresses, duplicate participant enrolments in the same category, and multiple results for the same enrolment.

## 6. Authentication and User Roles

The API includes authentication functionality for user registration and login.

The planned user roles are:

- **Admin:** Responsible for system administration.
- **Organiser:** Responsible for managing race events.
- **Participant:** Registers for events and participates in races.

Access to protected endpoints should be controlled through authentication and role-based authorisation where implemented.

## 7. Project Structure

The main project files and folders include:

```text
RaceDayAPI/
├── Controllers/
├── Data/
│   └── RaceDayContext.cs
├── Models/
├── Properties/
├── Program.cs
├── appsettings.json
├── RaceDayAPI.csproj
└── README.md
```

*Note: The actual folder structure may differ depending on the files and migrations included in your project.*

## 8. How to Run the Project

1. Install Visual Studio and the .NET SDK version required by the project.
2. Install and configure Microsoft SQL Server.
3. Clone or download this repository.
4. Open `RaceDayAPI.sln` in Visual Studio, if the solution file is included. Otherwise, open `RaceDayAPI.csproj`.
5. Configure the database connection string for your local SQL Server instance.
6. Configure any required authentication settings, including the JWT signing key, using secure development configuration.
7. Restore the NuGet packages.
8. Apply the Entity Framework Core migrations if they are included in the project, or create the database schema using the project's configured database setup.
9. Build the solution.
10. Run the application and open the Swagger page if Swagger is enabled.

The exact database setup steps depend on the migrations and configuration included in the repository.

## 9. API Testing

Swagger / OpenAPI can be used to inspect and test available API endpoints.

Depending on the endpoints implemented, testing may include:

- Registering and logging in users.
- Creating, viewing, updating, and deleting events.
- Managing categories and enrolments.
- Retrieving and managing race results.
- Testing authentication on protected endpoints.

## 10. Security Considerations

- Passwords should be stored as secure password hashes rather than plain text.
- JWT signing keys and database passwords must not be committed to GitHub.
- Protected endpoints should require appropriate authentication and authorisation.
- Input validation should be applied to incoming API requests.
- Database access should use parameterised operations through Entity Framework Core.

## 11. Project Status

This repository contains the source code for the RaceDayAPI project. Features should be considered complete only when they have been implemented and tested successfully.

## 12. Author

**Project:** RaceDayAPI – Race Day Management System  
**Purpose:** Race event, participant, enrolment, and result management.

---

Thank you for viewing the RaceDayAPI project.

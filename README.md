Employee & Project Management System

A console-based Employee and Project Management System built with C#, .NET 9, Entity Framework Core 9, SQLite, and Code First.

## Project Overview

This application is designed to manage employees, departments, and projects while handling their relationships efficiently.

## Features

### Employee Management
- Add employees
- Edit employee data
- Assign employees to departments
- Assign employees to projects
- Remove employees from projects
- Delete employees
- Display employee information with department and projects

### Department Management
- Add departments
- Edit department data
- Assign employees to departments
- Delete departments
- Display departments with their employees

### Project Management
- Add projects
- Edit project data
- Assign employees to projects
- Delete projects
- Display projects with their employees

## Technologies

- C#
- .NET 9
- Entity Framework Core 9
- SQLite
- Code First
- LINQ
- Console Application

## Database

The project uses SQLite as the database.

Database file:

`CompanyDB.db`

Entity Framework Core Code First is used to create and manage the database structure.

## Relationships

```text
Department (1) -------- (Many) Employee

Employee (Many) -------- (Many) Project
The many-to-many relationship between Employee and Project is handled through the EmployeeProject entity.
Project Structure
EmployeeProjectManagement_EF9_Agwa
│
├── Migrations
│
├── Models
│   ├── Department.cs
│   ├── Employee.cs
│   ├── Project.cs
│   ├── EmployeeProject.cs
│   └── CompanyDbContext.cs
│
├── Program.cs
├── CompanyDB.db
├── EmployeeProjectManagement_EF9_Agwa.csproj
└── EmployeeProjectManagement_EF9_Agwa.sln
How to Run
Clone the repository.
Open the solution in Visual Studio.
Restore the NuGet packages.
Build the project.
Run the application.
NuGet Packages
Microsoft.EntityFrameworkCore.Sqlite
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
Application Menu
EMPLOYEE / PROJECT MANAGEMENT

1. Add
2. Edit
3. Delete
4. Display
5. Exit
Requirements
Visual Studio 2022
.NET 9 SDK
SQLite
Entity Framework Core 9
Learning Objectives
This project demonstrates practical implementation of:
Entity Framework Core
Code First approach
SQLite database
CRUD operations
One-to-many relationships
Many-to-many relationships
LINQ queries
Navigation properties
Entity relationships
Author
Ahmed Agwa

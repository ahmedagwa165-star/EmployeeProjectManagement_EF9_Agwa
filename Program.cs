using Microsoft.EntityFrameworkCore;
using EmployeeProjectManagement_EF9_Agwa.Models;

namespace EmployeeProjectManagement_EF9_Agwa;

class Program
{
    static void Main()
    {
        using var db = new CompanyDbContext();

        db.Database.EnsureCreated();
        Seed(db);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("EMPLOYEE / PROJECT MANAGEMENT");
            Console.WriteLine("1. Add");
            Console.WriteLine("2. Edit");
            Console.WriteLine("3. Delete");
            Console.WriteLine("4. Display");
            Console.WriteLine("5. Exit");
            Console.Write("Choose: ");

            switch (Console.ReadLine())
            {
                case "1": Add(db); break;
                case "2": Edit(db); break;
                case "3": Delete(db); break;
                case "4": Display(db); break;
                case "5": return;
            }
        }
    }

    static void Add(CompanyDbContext db)
    {
        Console.Clear();
        Console.WriteLine("1. Employee");
        Console.WriteLine("2. Department");
        Console.WriteLine("3. Project");
        Console.Write("Choose: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Write("First Name: ");
                var first = Console.ReadLine() ?? "";
                Console.Write("Last Name: ");
                var last = Console.ReadLine() ?? "";

                var departments = db.Departments.OrderBy(d => d.Name).ToList();
                ShowDepartments(departments);

                Console.Write("Choose Department: ");
                if (GetChoice(departments.Count, out int d))
                {
                    db.Employees.Add(new Employee
                    {
                        FirstName = first,
                        LastName = last,
                        DepartmentId = departments[d].DepartmentId
                    });
                    db.SaveChanges();
                    Console.WriteLine("Employee added.");
                }
                Pause();
                break;

            case "2":
                Console.Write("Department Name: ");
                var departmentName = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(departmentName))
                {
                    db.Departments.Add(new Department { Name = departmentName });
                    db.SaveChanges();
                    Console.WriteLine("Department added.");
                }
                Pause();
                break;

            case "3":
                Console.Write("Project Name: ");
                var projectName = Console.ReadLine() ?? "";

                Console.Write("Start Date (yyyy-MM-dd): ");
                if (!DateOnly.TryParse(Console.ReadLine(), out var start))
                {
                    Console.WriteLine("Invalid date.");
                    Pause();
                    return;
                }

                Console.Write("End Date (yyyy-MM-dd) or Enter: ");
                var endText = Console.ReadLine();

                DateOnly? end = null;

                if (!string.IsNullOrWhiteSpace(endText))
                {
                    if (!DateOnly.TryParse(endText, out var endDate))
                    {
                        Console.WriteLine("Invalid date.");
                        Pause();
                        return;
                    }

                    end = endDate;
                }

                db.Projects.Add(new Project
                {
                    Name = projectName,
                    StartDate = start,
                    EndDate = end
                });

                db.SaveChanges();
                Console.WriteLine("Project added.");
                Pause();
                break;
        }
    }

    static void Edit(CompanyDbContext db)
    {
        Console.Clear();
        Console.WriteLine("1. Employee");
        Console.WriteLine("2. Department");
        Console.WriteLine("3. Project");
        Console.Write("Choose: ");

        switch (Console.ReadLine())
        {
            case "1": EditEmployee(db); break;
            case "2": EditDepartment(db); break;
            case "3": EditProject(db); break;
        }
    }

    static void EditEmployee(CompanyDbContext db)
    {
        var employees = db.Employees
            .Include(e => e.Department)
            .OrderBy(e => e.FirstName)
            .ToList();

        ShowEmployees(employees);

        Console.Write("Choose Employee: ");

        if (!GetChoice(employees.Count, out int choice))
        {
            Pause();
            return;
        }

        var employee = employees[choice];

        Console.Clear();
        Console.WriteLine($"Name: {employee.FirstName} {employee.LastName}");
        Console.WriteLine($"Department: {employee.Department.Name}");
        Console.WriteLine();

        Console.WriteLine("1. Edit Data");
        Console.WriteLine("2. Change Department");
        Console.WriteLine("3. Add Project");
        Console.WriteLine("4. Remove Project");
        Console.Write("Choose: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Write($"First Name [{employee.FirstName}]: ");
                var first = Console.ReadLine();

                Console.Write($"Last Name [{employee.LastName}]: ");
                var last = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(first))
                    employee.FirstName = first;

                if (!string.IsNullOrWhiteSpace(last))
                    employee.LastName = last;

                db.SaveChanges();
                Console.WriteLine("Employee updated.");
                break;

            case "2":
                var departments = db.Departments.OrderBy(d => d.Name).ToList();
                ShowDepartments(departments);

                Console.Write("Choose Department: ");

                if (GetChoice(departments.Count, out int d))
                {
                    employee.DepartmentId = departments[d].DepartmentId;
                    db.SaveChanges();
                    Console.WriteLine("Department changed.");
                }
                break;

            case "3":
                AddProjectToEmployee(db, employee);
                break;

            case "4":
                RemoveProjectFromEmployee(db, employee);
                break;
        }

        Pause();
    }

    static void AddProjectToEmployee(
        CompanyDbContext db,
        Employee employee)
    {
        var projects = db.Projects.ToList();

        var assigned = db.EmployeeProjects
            .Where(x => x.EmployeeId == employee.EmployeeId)
            .Select(x => x.ProjectId)
            .ToList();

        var available = projects
            .Where(x => !assigned.Contains(x.ProjectId))
            .OrderBy(x => x.Name)
            .ToList();

        for (int i = 0; i < available.Count; i++)
            Console.WriteLine($"{i + 1}. {available[i].Name}");

        Console.Write("Choose Project: ");

        if (!GetChoice(available.Count, out int choice))
            return;

        Console.Write("Role: ");
        var role = Console.ReadLine() ?? "";

        db.EmployeeProjects.Add(new EmployeeProject
        {
            EmployeeId = employee.EmployeeId,
            ProjectId = available[choice].ProjectId,
            Role = role
        });

        db.SaveChanges();
        Console.WriteLine("Project assigned.");
    }

    static void RemoveProjectFromEmployee(
        CompanyDbContext db,
        Employee employee)
    {
        var projects = db.EmployeeProjects
            .Include(x => x.Project)
            .Where(x => x.EmployeeId == employee.EmployeeId)
            .ToList();

        for (int i = 0; i < projects.Count; i++)
            Console.WriteLine($"{i + 1}. {projects[i].Project.Name}");

        Console.Write("Choose Project: ");

        if (!GetChoice(projects.Count, out int choice))
            return;

        db.EmployeeProjects.Remove(projects[choice]);
        db.SaveChanges();

        Console.WriteLine("Project removed.");
    }

    static void EditDepartment(CompanyDbContext db)
    {
        var departments = db.Departments
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToList();

        ShowDepartments(departments);

        Console.Write("Choose Department: ");

        if (!GetChoice(departments.Count, out int choice))
        {
            Pause();
            return;
        }

        var department = departments[choice];

        Console.Clear();
        Console.WriteLine($"Current Name: {department.Name}");

        Console.Write("New Name: ");
        var name = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(name))
            department.Name = name;

        Console.WriteLine();
        Console.WriteLine("1. Save");
        Console.WriteLine("2. Assign Employee");
        Console.Write("Choose: ");

        if (Console.ReadLine() == "2")
        {
            var employees = db.Employees.ToList();
            ShowEmployees(employees);

            Console.Write("Choose Employee: ");

            if (GetChoice(employees.Count, out int e))
                employees[e].DepartmentId = department.DepartmentId;
        }

        db.SaveChanges();
        Console.WriteLine("Department updated.");
        Pause();
    }

    static void EditProject(CompanyDbContext db)
    {
        var projects = db.Projects
            .OrderBy(p => p.Name)
            .ToList();

        ShowProjects(projects);

        Console.Write("Choose Project: ");

        if (!GetChoice(projects.Count, out int choice))
        {
            Pause();
            return;
        }

        var project = projects[choice];

        Console.Clear();
        Console.WriteLine($"Name: {project.Name}");
        Console.WriteLine($"Start: {project.StartDate}");
        Console.WriteLine($"End: {project.EndDate?.ToString() ?? "Not Finished"}");
        Console.WriteLine();

        Console.Write("New Name: ");
        var name = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(name))
            project.Name = name;

        Console.Write("New Start Date or Enter: ");
        var startText = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(startText) &&
            DateOnly.TryParse(startText, out var start))
            project.StartDate = start;

        Console.WriteLine();
        Console.WriteLine("1. Save");
        Console.WriteLine("2. Assign Employee");
        Console.Write("Choose: ");

        if (Console.ReadLine() == "2")
        {
            var employees = db.Employees.ToList();
            ShowEmployees(employees);

            Console.Write("Choose Employee: ");

            if (GetChoice(employees.Count, out int e))
            {
                Console.Write("Role: ");
                var role = Console.ReadLine() ?? "";

                db.EmployeeProjects.Add(new EmployeeProject
                {
                    EmployeeId = employees[e].EmployeeId,
                    ProjectId = project.ProjectId,
                    Role = role
                });
            }
        }

        db.SaveChanges();
        Console.WriteLine("Project updated.");
        Pause();
    }

    static void Delete(CompanyDbContext db)
    {
        Console.Clear();
        Console.WriteLine("1. Employee");
        Console.WriteLine("2. Department");
        Console.WriteLine("3. Project");
        Console.Write("Choose: ");

        switch (Console.ReadLine())
        {
            case "1":
                var employees = db.Employees.ToList();
                ShowEmployees(employees);

                Console.Write("Choose Employee: ");

                if (GetChoice(employees.Count, out int e))
                {
                    db.Employees.Remove(employees[e]);
                    db.SaveChanges();
                    Console.WriteLine("Employee deleted.");
                }
                break;

            case "2":
                var departments = db.Departments.ToList();
                ShowDepartments(departments);

                Console.Write("Choose Department: ");

                if (GetChoice(departments.Count, out int d))
                {
                    db.Departments.Remove(departments[d]);
                    db.SaveChanges();
                    Console.WriteLine("Department deleted.");
                }
                break;

            case "3":
                var projects = db.Projects.ToList();
                ShowProjects(projects);

                Console.Write("Choose Project: ");

                if (GetChoice(projects.Count, out int p))
                {
                    db.Projects.Remove(projects[p]);
                    db.SaveChanges();
                    Console.WriteLine("Project deleted.");
                }
                break;
        }

        Pause();
    }

    static void Display(CompanyDbContext db)
    {
        Console.Clear();
        Console.WriteLine("1. Employees");
        Console.WriteLine("2. Departments");
        Console.WriteLine("3. Projects");
        Console.Write("Choose: ");

        switch (Console.ReadLine())
        {
            case "1":
                foreach (var e in db.Employees
                    .Include(e => e.Department)
                    .Include(e => e.EmployeeProjects)
                    .ThenInclude(x => x.Project))
                {
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"{e.FirstName} {e.LastName}");
                    Console.WriteLine($"Department: {e.Department.Name}");

                    foreach (var p in e.EmployeeProjects)
                        Console.WriteLine($"Project: {p.Project.Name} - {p.Role}");
                }
                break;

            case "2":
                foreach (var d in db.Departments.Include(d => d.Employees))
                {
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"Department: {d.Name}");

                    foreach (var e in d.Employees)
                        Console.WriteLine($"Employee: {e.FirstName} {e.LastName}");
                }
                break;

            case "3":
                foreach (var p in db.Projects
                    .Include(p => p.EmployeeProjects)
                    .ThenInclude(x => x.Employee))
                {
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"Project: {p.Name}");
                    Console.WriteLine($"Start: {p.StartDate}");
                    Console.WriteLine($"End: {p.EndDate?.ToString() ?? "Not Finished"}");

                    foreach (var e in p.EmployeeProjects)
                        Console.WriteLine(
                            $"Employee: {e.Employee.FirstName} {e.Employee.LastName} - {e.Role}");
                }
                break;
        }

        Pause();
    }

    static void ShowDepartments(List<Department> list)
    {
        for (int i = 0; i < list.Count; i++)
            Console.WriteLine($"{i + 1}. {list[i].Name}");
    }

    static void ShowEmployees(List<Employee> list)
    {
        for (int i = 0; i < list.Count; i++)
            Console.WriteLine($"{i + 1}. {list[i].FirstName} {list[i].LastName}");
    }

    static void ShowProjects(List<Project> list)
    {
        for (int i = 0; i < list.Count; i++)
            Console.WriteLine($"{i + 1}. {list[i].Name}");
    }

    static bool GetChoice(int count, out int choice)
    {
        if (int.TryParse(Console.ReadLine(), out int number) &&
            number >= 1 &&
            number <= count)
        {
            choice = number - 1;
            return true;
        }

        choice = -1;
        Console.WriteLine("Invalid choice.");
        return false;
    }

    static void Seed(CompanyDbContext db)
    {
        if (db.Departments.Any())
            return;

        var engineering = new Department { Name = "Engineering" };
        var marketing = new Department { Name = "Marketing" };
        var finance = new Department { Name = "Finance" };

        db.Departments.AddRange(engineering, marketing, finance);
        db.SaveChanges();

        var john = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            DepartmentId = engineering.DepartmentId
        };

        var jane = new Employee
        {
            FirstName = "Jane",
            LastName = "Smith",
            DepartmentId = engineering.DepartmentId
        };

        var mike = new Employee
        {
            FirstName = "Mike",
            LastName = "Johnson",
            DepartmentId = marketing.DepartmentId
        };

        var sarah = new Employee
        {
            FirstName = "Sarah",
            LastName = "Williams",
            DepartmentId = finance.DepartmentId
        };

        db.Employees.AddRange(john, jane, mike, sarah);
        db.SaveChanges();

        var website = new Project
        {
            Name = "Website Redesign",
            StartDate = new DateOnly(2023, 1, 15),
            EndDate = new DateOnly(2023, 6, 30)
        };

        var mobile = new Project
        {
            Name = "Mobile App Development",
            StartDate = new DateOnly(2023, 2, 1)
        };

        var financial = new Project
        {
            Name = "Financial System Upgrade",
            StartDate = new DateOnly(2023, 3, 10),
            EndDate = new DateOnly(2023, 12, 15)
        };

        db.Projects.AddRange(website, mobile, financial);
        db.SaveChanges();

        db.EmployeeProjects.AddRange(
            new EmployeeProject
            {
                EmployeeId = john.EmployeeId,
                ProjectId = website.ProjectId,
                Role = "Lead Developer"
            },
            new EmployeeProject
            {
                EmployeeId = john.EmployeeId,
                ProjectId = mobile.ProjectId,
                Role = "Architect"
            },
            new EmployeeProject
            {
                EmployeeId = jane.EmployeeId,
                ProjectId = website.ProjectId,
                Role = "Frontend Developer"
            },
            new EmployeeProject
            {
                EmployeeId = jane.EmployeeId,
                ProjectId = mobile.ProjectId,
                Role = "Backend Developer"
            },
            new EmployeeProject
            {
                EmployeeId = mike.EmployeeId,
                ProjectId = website.ProjectId,
                Role = "UX Designer"
            },
            new EmployeeProject
            {
                EmployeeId = sarah.EmployeeId,
                ProjectId = financial.ProjectId,
                Role = "Project Manager"
            });

        db.SaveChanges();
    }

    static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }
}
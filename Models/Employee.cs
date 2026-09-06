namespace EmployeeProjectManagement_EF9_Agwa.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
}
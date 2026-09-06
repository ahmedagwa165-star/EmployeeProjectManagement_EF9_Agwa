namespace EmployeeProjectManagement_EF9_Agwa.Models;

public class EmployeeProject
{
    public int EmployeeId { get; set; }

    public int ProjectId { get; set; }

    public string Role { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public Project Project { get; set; } = null!;
}
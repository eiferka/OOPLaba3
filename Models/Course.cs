namespace Lab3Api.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Hours { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
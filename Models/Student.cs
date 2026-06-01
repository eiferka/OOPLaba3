namespace Lab3Api.Models;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public int GroupId { get; set; }
    public Group? Group { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
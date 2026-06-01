namespace Lab3Api.Models;

public class Group
{
    public int Id { get; set; }
    public string Direction { get; set; } = string.Empty;
    public int AdmissionYear { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
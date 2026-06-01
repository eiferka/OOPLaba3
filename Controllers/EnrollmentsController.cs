using Lab3Api.Data;
using Lab3Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnrollmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public EnrollmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (enrollment == null)
            return NotFound();

        return enrollment;
    }

    [HttpPost]
    public async Task<ActionResult<Enrollment>> CreateEnrollment(Enrollment enrollment)
    {
        var studentExists = await _context.Students.AnyAsync(s => s.Id == enrollment.StudentId);
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollment.CourseId);

        if (!studentExists)
            return BadRequest("Студент с таким Id не найден.");

        if (!courseExists)
            return BadRequest("Курс с таким Id не найден.");

        var alreadyExists = await _context.Enrollments.AnyAsync(e =>
            e.StudentId == enrollment.StudentId &&
            e.CourseId == enrollment.CourseId);

        if (alreadyExists)
            return BadRequest("Студент уже записан на этот курс.");

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEnrollment), new { id = enrollment.Id }, enrollment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEnrollment(int id, Enrollment enrollment)
    {
        if (id != enrollment.Id)
            return BadRequest();

        var studentExists = await _context.Students.AnyAsync(s => s.Id == enrollment.StudentId);
        var courseExists = await _context.Courses.AnyAsync(c => c.Id == enrollment.CourseId);

        if (!studentExists)
            return BadRequest("Студент с таким Id не найден.");

        if (!courseExists)
            return BadRequest("Курс с таким Id не найден.");

        _context.Entry(enrollment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnrollment(int id)
    {
        var enrollment = await _context.Enrollments.FindAsync(id);

        if (enrollment == null)
            return NotFound();

        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
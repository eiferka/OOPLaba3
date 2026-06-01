using Lab3Api.Data;
using Lab3Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab3Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
    {
        return await _context.Groups
            .Include(g => g.Students)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Group>> GetGroup(int id)
    {
        var group = await _context.Groups
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            return NotFound();

        return group;
    }

    [HttpPost]
    public async Task<ActionResult<Group>> CreateGroup(Group group)
    {
        if (string.IsNullOrWhiteSpace(group.Direction))
            return BadRequest("Направление не может быть пустым.");

        if (string.IsNullOrWhiteSpace(group.Name))
            return BadRequest("Имя группы не может быть пустым.");

        if (group.AdmissionYear < 0 || group.AdmissionYear > 99)
            return BadRequest("Год поступления должен быть в формате от 0 до 99. Например: 24.");

        var alreadyExists = await _context.Groups.AnyAsync(g => g.Name == group.Name);

        if (alreadyExists)
            return BadRequest("Группа с таким именем уже существует.");

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(int id, Group group)
    {
        if (id != group.Id)
            return BadRequest();

        if (string.IsNullOrWhiteSpace(group.Direction))
            return BadRequest("Направление не может быть пустым.");

        if (string.IsNullOrWhiteSpace(group.Name))
            return BadRequest("Имя группы не может быть пустым.");

        if (group.AdmissionYear < 0 || group.AdmissionYear > 99)
            return BadRequest("Год поступления должен быть в формате от 0 до 99. Например: 24.");

        var alreadyExists = await _context.Groups.AnyAsync(g =>
            g.Id != id &&
            g.Name == group.Name);

        if (alreadyExists)
            return BadRequest("Группа с таким именем уже существует.");

        _context.Entry(group).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        var group = await _context.Groups.FindAsync(id);

        if (group == null)
            return NotFound();

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
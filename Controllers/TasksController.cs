using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {

        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET TASKS (ADMIN = ALL TASKS, EMPLOYEE = OWN TASKS)
        [HttpGet]
        public async Task<IActionResult> GetTasks(int? userId)
        {
            var query = from task in _context.Tasks
                        join user in _context.Users
                        on task.AssignedTo equals user.UserId
                        select new
                        {
                            task.TaskId,
                            task.Title,
                            task.Description,
                            task.Status,
                            task.DueDate,
                            task.CreatedDate,
                            task.AssignedTo,
                            AssignedName = user.Name,
                            AssignedEmail = user.Email
                        };

            if (userId.HasValue)
            {
                query = query.Where(x => x.AssignedTo == userId.Value);
            }

            var result = await query.ToListAsync();

            return Ok(result);
        }

        // CREATE TASK
        [HttpPost]
        public async Task<IActionResult> AddTask(TaskItem task)
        {
            task.CreatedDate = DateTime.Now;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return Ok(task);
        }

        // UPDATE TASK
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem task)
        {
            var existingTask = await _context.Tasks.FindAsync(id);

            if (existingTask == null)
                return NotFound();

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Status = task.Status;
            existingTask.DueDate = task.DueDate;
            existingTask.AssignedTo = task.AssignedTo;

            await _context.SaveChangesAsync();

            return Ok(existingTask);
        }

        // DELETE TASK
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
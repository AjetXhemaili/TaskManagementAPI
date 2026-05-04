using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Shared;
using TaskItem = TaskManagementAPI.Models.TaskItem;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context) => _context = context;

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = GetCurrentUserId();

            // Fix: use the DbSet name that matches the ApplicationDbContext (commonly plural).
                  var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    UserId = t.UserId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToListAsync();

            return Ok(new ApiResponse<List<TaskResponseDto>>
            {
                Success = true,
                Message = "Tasks retrieved successfully.",
                Data = tasks
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found or access denied." });

            var dto = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate,
                Priority = task.Priority,
                UserId = task.UserId,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };

            return Ok(new ApiResponse<TaskResponseDto>
            {
                Success = true,
                Message = "Task retrieved successfully.",
                Data = dto
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var userId = GetCurrentUserId();
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new ApiResponse<TaskResponseDto>
            {
                Success = true,
                Message = "Task created successfully.",
                Data = new TaskResponseDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    UserId = task.UserId,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found or access denied." });

            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.IsCompleted.HasValue) task.IsCompleted = dto.IsCompleted.Value;
            if (dto.DueDate.HasValue) task.DueDate = dto.DueDate.Value;
            if (dto.Priority != null) task.Priority = dto.Priority;

            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Task not found or access denied." });

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Task deleted successfully."
            });
        }
    }
}
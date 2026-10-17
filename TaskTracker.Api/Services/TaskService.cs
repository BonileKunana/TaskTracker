
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Services;

public class TaskService : ITaskService
{
    private readonly TaskDbContext _db;

    public TaskService(TaskDbContext db)
    {
        _db = db;
    }

    public async Task<List<TaskResponse>> GetAllAsync()
    {
        return await _db.Tasks
            .Select(task => new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted
            })
            .ToListAsync();
    }

    public async Task<TaskResponse?> GetByIdAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task is null)
        {
            return null;
        }

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted
        };
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = request.IsCompleted
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted
        };
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateTaskRequest request)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task is null)
        {
            return false;
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task is null)
        {
            return false;
        }

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return true;
    }
}

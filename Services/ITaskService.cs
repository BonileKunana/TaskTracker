
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Services;

public interface ITaskService
{
    Task<List<TaskItem>> GetAllAsync();

    Task<TaskItem?> GetByIdAsync(int id);

    Task<TaskItem> CreateAsync(TaskItem task);

    Task<bool> UpdateAsync(int id, TaskItem updatedTask);

    Task<bool> DeleteAsync(int id);
}

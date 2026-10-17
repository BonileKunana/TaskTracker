
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(
        DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
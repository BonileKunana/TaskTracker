
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Services;
using TaskTracker.Api.Dtos;

namespace TaskTracker.Api.Tests;

public class TaskServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new TaskDbContext(options);
        var service = new TaskService(db);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsTask()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new TaskDbContext(options);

        var task = new TaskTracker.Api.Models.TaskItem
        {
            Id = 1,
            Title = "Learn unit testing",
            Description = "Write my first tests",
            IsCompleted = false
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TaskService(db);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Learn unit testing", result.Title);
        Assert.Equal("Write my first tests", result.Description);
        Assert.False(result.IsCompleted);
    }


    [Fact]
    public async Task CreateAsync_WithValidRequest_CreatesAndReturnsTask()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new TaskDbContext(options);
        var service = new TaskService(db);

        var request = new CreateTaskRequest
        {
            Title = "Learn automated testing",
            Description = "Test task creation",
            IsCompleted = false
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.False(result.IsCompleted);

        var savedTask = await db.Tasks.FindAsync(result.Id);
        Assert.NotNull(savedTask);
        Assert.Equal(request.Title, savedTask.Title);
    }



    [Fact]
    public async Task UpdateAsync_WhenTaskExists_UpdatesTask()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new TaskDbContext(options);

        var task = new TaskTracker.Api.Models.TaskItem
        {
            Title = "Old title",
            Description = "Old description",
            IsCompleted = false
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TaskService(db);

        var request = new UpdateTaskRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            IsCompleted = true
        };

        // Act
        var result = await service.UpdateAsync(task.Id, request);

        // Assert
        Assert.True(result);

        var updatedTask = await db.Tasks.FindAsync(task.Id);

        Assert.NotNull(updatedTask);
        Assert.Equal("Updated title", updatedTask.Title);
        Assert.Equal("Updated description", updatedTask.Description);
        Assert.True(updatedTask.IsCompleted);
    }


    [Fact]
    public async Task DeleteAsync_WhenTaskExists_DeletesTask()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new TaskDbContext(options);

        var task = new TaskTracker.Api.Models.TaskItem
        {
            Title = "Task to delete",
            Description = "Testing deletion",
            IsCompleted = false
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        var service = new TaskService(db);

        // Act
        var result = await service.DeleteAsync(task.Id);

        // Assert
        Assert.True(result);

        var deletedTask = await db.Tasks.FindAsync(task.Id);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new TaskDbContext(options);
        var service = new TaskService(db);

        // Act
        var result = await service.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenTasksExist_ReturnsAllTasks()
    {
        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var db = new TaskDbContext(options);

        db.Tasks.AddRange(
            new TaskTracker.Api.Models.TaskItem
            {
                Title = "Learn EF Core",
                Description = "Understand database access",
                IsCompleted = false
            },
            new TaskTracker.Api.Models.TaskItem
            {
                Title = "Learn unit testing",
                Description = "Test the service",
                IsCompleted = true
            }
        );

        await db.SaveChangesAsync();

        var service = new TaskService(db);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, task => task.Title == "Learn EF Core");
        Assert.Contains(result, task => task.Title == "Learn unit testing");
    }

}


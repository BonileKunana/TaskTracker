using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskTracker.Api.Data;
using System.Net;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using TaskTracker.Api.Models;
using TaskTracker.Api.Dtos;
using System.Net.Http.Json;

namespace TaskTracker.Api.Tests;

public class TaskApiIntegrationTests
    : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(
    IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        "Host=localhost;Database=test;Username=test;Password=test"
                });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                Microsoft.EntityFrameworkCore.Infrastructure
                    .IDbContextOptionsConfiguration<TaskDbContext>>();

            services.RemoveAll<DbContextOptions<TaskDbContext>>();
            services.RemoveAll<TaskDbContext>();

            services.AddDbContext<TaskDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    [Fact]
    public async Task GetTasks_ReturnsOk()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/tasks");
        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {(int)response.StatusCode}");
        Console.WriteLine($"Response body: {body}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/tasks/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTask_WhenTaskExists_ReturnsOk()
    {
        // Arrange: insert a task into the API's test database
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaskDbContext>();

        var task = new TaskItem
        {
            Title = "Integration test task",
            Description = "Testing GET by ID",
            IsCompleted = false
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        // Act: request the task through HTTP
        using var client = CreateClient();

        var response = await client.GetAsync($"/tasks/{task.Id}");

        // Assert: verify the status and response data
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
        Assert.Equal("Integration test task", result.Title);
    }

    [Fact]
    public async Task CreateTask_WithValidRequest_ReturnsCreated()
    {
        using var client = CreateClient();

        var request = new CreateTaskRequest
        {
            Title = "Integration test task",
            Description = "Created through HTTP",
            IsCompleted = false
        };

        var response = await client.PostAsJsonAsync(
            "/tasks", request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var result = await response.Content
            .ReadFromJsonAsync<TaskResponse>();

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
    }

    [Fact]
    public async Task CreateTask_WithInvalidTitle_ReturnsBadRequest()
    {
        using var client = CreateClient();

        var request = new CreateTaskRequest
        {
            Title = "AB",
            Description = "Testing invalid input",
            IsCompleted = false
        };

        var response = await client.PostAsJsonAsync(
            "/tasks", request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_WhenTaskExists_ReturnsNoContent()
    {
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaskDbContext>();

        var task = new TaskItem
        {
            Title = "Original title",
            Description = "Original description",
            IsCompleted = false
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        using var client = CreateClient();

        var request = new UpdateTaskRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            IsCompleted = true
        };

        var response = await client.PutAsJsonAsync(
            $"/tasks/{task.Id}", request);

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        using var client = CreateClient();

        var request = new UpdateTaskRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            IsCompleted = true
        };

        var response = await client.PutAsJsonAsync(
            "/tasks/999", request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_WhenTaskExists_ReturnsNoContent()
    {
        using var scope = Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<TaskDbContext>();

        var task = new TaskItem
        {
            Title = "Task to delete",
            Description = "Testing HTTP deletion",
            IsCompleted = false
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        using var client = CreateClient();

        var response = await client.DeleteAsync(
            $"/tasks/{task.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        using var client = CreateClient();

        var response = await client.DeleteAsync("/tasks/999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    [Fact]
    public async Task CreateTask_WithTooLongDescription_ReturnsBadRequest()
    {
        using var client = CreateClient();

        var request = new CreateTaskRequest
        {
            Title = "Valid task title",
            Description = new string('A', 501),
            IsCompleted = false
        };

        var response = await client.PostAsJsonAsync(
            "/tasks", request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_WithInvalidTitle_ReturnsBadRequest()
    {
        using var client = CreateClient();

        var request = new UpdateTaskRequest
        {
            Title = "AB",
            Description = "Valid description",
            IsCompleted = false
        };

        var response = await client.PutAsJsonAsync(
            "/tasks/1", request);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

}
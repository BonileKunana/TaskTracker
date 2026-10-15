using TaskTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is missing.");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITaskService, TaskService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.MapGet("/tasks", async (ITaskService service) =>
{
    var tasks = await service.GetAllAsync();
    return Results.Ok(tasks);
});

app.MapPost("/tasks", async (TaskItem task, ITaskService service) =>
{
    var createdTask = await service.CreateAsync(task);

    return Results.Created($"/tasks/{createdTask.Id}", createdTask);
});

app.MapGet("/tasks/{id:int}", async (int id, ITaskService service) =>
{
    var task = await service.GetByIdAsync(id);

    return task is null
        ? Results.NotFound()
        : Results.Ok(task);
});

app.MapPut("/tasks/{id:int}", async (int id, TaskItem updatedTask, ITaskService service) =>
{
    var updated = await service.UpdateAsync(id, updatedTask);

    return updated
        ? Results.NoContent()
        : Results.NotFound();
});

app.MapDelete("/tasks/{id:int}", async (int id, ITaskService service) =>
{
    var deleted = await service.DeleteAsync(id);

    return deleted
        ? Results.NoContent()
        : Results.NotFound();
});


app.Run();



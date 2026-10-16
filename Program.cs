using TaskTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Models;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Validation;

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


app.MapPost("/tasks", async (
    CreateTaskRequest request,
    ITaskService service) =>
{
    var errors = DtoValidator.Validate(request);

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var createdTask = await service.CreateAsync(request);

    return Results.Created(
        $"/tasks/{createdTask.Id}",
        createdTask);
});


app.MapGet("/tasks/{id:int}", async (
    int id,
    ITaskService service) =>
{
    var task = await service.GetByIdAsync(id);

    return task is not null
        ? Results.Ok(task)
        : Results.NotFound();
});


app.MapPut("/tasks/{id:int}", async (
    int id,
    UpdateTaskRequest request,
    ITaskService service) =>
{
    var errors = DtoValidator.Validate(request);

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var updated = await service.UpdateAsync(id, request);

    return updated
        ? Results.NoContent()
        : Results.NotFound();
});


app.MapDelete("/tasks/{id:int}", async (
    int id,
    ITaskService service) =>
{
    var deleted = await service.DeleteAsync(id);

    return deleted
        ? Results.NoContent()
        : Results.NotFound();
});


app.Run();



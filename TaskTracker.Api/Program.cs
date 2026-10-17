
using TaskTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Dtos;
using TaskTracker.Api.Validation;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is missing.");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("TaskTrackerFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5500",
                "https://tasktracker-web-kjg6.onrender.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

// Log health-check failures without returning sensitive details to callers.
builder.Logging.AddFilter(
    "Microsoft.Extensions.Diagnostics.HealthChecks",
    LogLevel.Information);

var app = builder.Build();

app.UseCors("TaskTrackerFrontend");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature =
            context.Features.Get<
                Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

        var logger = context.RequestServices
            .GetRequiredService<ILogger<Program>>();

        if (exceptionFeature?.Error is not null)
        {
            logger.LogError(
                exceptionFeature.Error,
                "An unhandled exception occurred while processing the request.");
        }

        context.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred."
        });
    });
});

app.MapHealthChecks(
    "/health",
    new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString()
                })
            });

            await context.Response.WriteAsync(result);
        }
    });

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

public partial class Program { }

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly TaskDbContext _db;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(
        TaskDbContext db,
        ILogger<DatabaseHealthCheck> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect =
                await _db.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                return HealthCheckResult.Healthy();
            }

            _logger.LogError(
                "Database health check failed: PostgreSQL connection returned false.");

            return HealthCheckResult.Unhealthy(
                "Database connection failed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Database health check failed. Exception type: {ExceptionType}; Message: {ExceptionMessage}",
                ex.GetType().Name,
                ex.Message);

            return HealthCheckResult.Unhealthy(
                "Database connection failed.");
        }
    }
}
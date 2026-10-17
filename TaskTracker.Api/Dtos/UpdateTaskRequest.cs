
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Api.Dtos;

public class UpdateTaskRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
}

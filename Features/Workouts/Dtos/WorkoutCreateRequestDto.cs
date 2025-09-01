using System.ComponentModel.DataAnnotations;

namespace C_Sharp_Web_API.Features.Workouts.Dtos;

public class WorkoutCreateRequestDto
{
    [Required(ErrorMessage = "A valid name has to be supplied.")]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace C_Sharp_Web_API.Features.Exercises.Dtos;

public class TemplateExerciseUpdateRequestDto
{
    [Required(ErrorMessage = "A valid name has to be supplied.")]
    public string Name { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "A valid muscle group has to be supplied.")]
    public string MuscleGroup { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "A valid unit has to be supplied.")]
    public string Unit { get; init; } = string.Empty;
}
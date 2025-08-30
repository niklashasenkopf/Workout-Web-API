using System.ComponentModel.DataAnnotations;

namespace C_Sharp_Web_API.Exercises.Models;

public class ExerciseCreateRequestDto
{
    [Required(ErrorMessage = "A valid name has to be supplied.")]
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}
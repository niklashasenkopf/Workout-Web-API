using System.ComponentModel.DataAnnotations;

namespace C_Sharp_Web_API.Features.WorkoutExercises.Dtos;

public class WorkoutExerciseUpdateRequestDto
{
    [Required]
    public int Order { get; set; }
    
    [Required]
    public int TemplateExerciseId { get; set; }
    
    [Required]
    public int WorkoutId { get; set; }
}
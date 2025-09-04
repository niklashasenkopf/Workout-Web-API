using System.ComponentModel.DataAnnotations;

namespace C_Sharp_Web_API.Features.TemplateExercises.Dtos;

public class TemplateExerciseCreateRequestDto
{
    [Required(ErrorMessage = "A valid name has to be supplied.")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string MuscleGroup { get; set; } = string.Empty;
    
    public Unit Unit { get; set; } = Unit.Kg;
}
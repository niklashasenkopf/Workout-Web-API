using C_Sharp_Web_API.Features.TemplateExercises;

namespace C_Sharp_Web_API.Features.WorkoutExercises.Dtos;

public class WorkoutExerciseDto
{
    public int Id { get; set; }
    public int Order { get; set; }
    public int TemplateExerciseId { get; set; }
    public TemplateExercise TemplateExercise { get; set; } = null!; 
    public int WorkoutId { get; set; }
}
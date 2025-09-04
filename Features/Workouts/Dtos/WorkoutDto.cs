using C_Sharp_Web_API.Features.WorkoutExercises;

namespace C_Sharp_Web_API.Features.Workouts.Dtos;

public class WorkoutDto
{
    public int Id { get; init; }
    
    public string Name { get; init; } = string.Empty;

    public ICollection<WorkoutExercise> Exercises { get; init; } = new List<WorkoutExercise>();
}
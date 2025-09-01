using C_Sharp_Web_API.Features.Exercises.Dtos;

namespace C_Sharp_Web_API.Features.Workouts.Dtos;

public class WorkoutDto
{
    public int Id { get; init; }
    
    public string Name { get; init; } = string.Empty;

    public ICollection<ExerciseWithoutSetEntriesDto> Exercises { get; init; } = new List<ExerciseWithoutSetEntriesDto>();
}
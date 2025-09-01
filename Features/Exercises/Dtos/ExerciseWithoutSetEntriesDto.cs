namespace C_Sharp_Web_API.Features.Exercises.Dtos;

public class ExerciseWithoutSetEntriesDto
{
    public int Id { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public string? MuscleGroup { get; init; }
}
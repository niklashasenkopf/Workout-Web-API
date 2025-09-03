using C_Sharp_Web_API.Features.SetEntries.Dtos;

namespace C_Sharp_Web_API.Features.Exercises.dtos;

public class TemplateExerciseDto 
{
    public int Id { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public string? MuscleGroup { get; init; }

    public ICollection<SetEntryDto> SetEntries { get; init; } = new List<SetEntryDto>();
    
    // Calculated field: Only present in the DTO to return, not in the entity
    public int NumberOfSetEntries => SetEntries.Count;
}
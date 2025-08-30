namespace C_Sharp_Web_API.Models;

public class ExerciseDto 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}
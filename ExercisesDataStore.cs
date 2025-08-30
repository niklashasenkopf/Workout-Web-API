using C_Sharp_Web_API.Models;

namespace C_Sharp_Web_API;

public class ExercisesDataStore
{
    public List<ExerciseDto> Exercises { get; set; }
    
    // Singleton pattern: 
    public static ExercisesDataStore Current { get; } = new ExercisesDataStore();

    public ExercisesDataStore()
    {
        // Initialize Dummy Data
        Exercises = new List<ExerciseDto>()
        {
            new ExerciseDto()
            {
                Id = 1,
                Name = "Bench Press",
                MuscleGroup = "Chest",
                Unit = "kg"
            },
            new ExerciseDto()
            {
                Id = 2,
                Name = "Lat Pulldown",
                MuscleGroup = "Back",
                Unit = "kg"
            },
            new ExerciseDto()
            {
                Id = 3,
                Name = "Preacher Curl",
                MuscleGroup = "Bicep",
                Unit = "kg"
            }
        };
    }
}
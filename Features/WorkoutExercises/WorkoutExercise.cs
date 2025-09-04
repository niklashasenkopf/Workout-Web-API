using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Features.SetEntries;
using C_Sharp_Web_API.Features.TemplateExercises;
using C_Sharp_Web_API.Features.Workouts;

namespace C_Sharp_Web_API.Features.WorkoutExercises;

public class WorkoutExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Range(0, int.MaxValue)]
    public int Order { get; init; }

    public ICollection<SetEntry> SetEntries = new List<SetEntry>();
    
    public int TemplateExerciseId { get; init; }
    public TemplateExercise TemplateExercise { get; init; } = null!;
    
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
}
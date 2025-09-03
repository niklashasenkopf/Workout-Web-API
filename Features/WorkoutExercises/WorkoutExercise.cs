using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.SetEntries.Domain;

namespace C_Sharp_Web_API.FeaturesNew.WorkoutExercises.Domain;

public class WorkoutExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Order { get; set; }

    public ICollection<SetEntry> SetEntries = new List<SetEntry>();
    
    public int TemplateExerciseId { get; set; }
    public TemplateExercise TemplateExercise { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Exercises.Entities;
using C_Sharp_Web_API.Template.WorkoutTemplates.Entities;

namespace C_Sharp_Web_API.Template.TemplateExercises.Entities;

public class TemplateExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int Position { get; set; }
    
    [Range(1, int.MaxValue)]
    public int? TargetReps { get; set; }
    
    [Range(1, int.MaxValue)]
    public int? TargetSets { get; set; }
    
    [Required]
    public int ExerciseId { get; set; }
    
    public Exercise Exercise { get; set; } = null!;
    
    [Required]
    public int WorkoutTemplateId { get; set; }

    public WorkoutTemplate WorkoutTemplate { get; set; } = null!; 
}
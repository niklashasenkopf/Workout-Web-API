using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Exercises.Entities;
using C_Sharp_Web_API.Session.WorkoutSessions.Entities;

namespace C_Sharp_Web_API.Session.SessionExercises.Entities;

public class SessionExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int Position { get; set; }
    
    [Required]
    public int WorkoutSessionId { get; set; }

    public WorkoutSession WorkoutSession { get; set; } = null!;
    
    [Required]
    public int ExerciseId { get; set; }

    public Exercise Exercise { get; set; } = null!;
}
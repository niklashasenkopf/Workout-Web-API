using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Authentication;
using C_Sharp_Web_API.Features.WorkoutExercises;

namespace C_Sharp_Web_API.Features.Workouts;

public class Workout(string name)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [Required, MaxLength(100)] 
    public string Name { get; init; } = name;

    public ICollection<WorkoutExercise> WorkoutExercises { get; init; } = new List<WorkoutExercise>();
    
    public Guid ApiUserId { get; set; }
    public ApiUser ApiUser { get; init; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Features.Exercises.Domain;

namespace C_Sharp_Web_API.Features.Workouts.Domain;

public class Workout(string name)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [Required] 
    [MaxLength(100)] 
    public string Name { get; init; } = name;

    public ICollection<Exercise> Exercises { get; init; } = new List<Exercise>();
}
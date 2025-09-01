using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Features.Exercises.Domain;

namespace C_Sharp_Web_API.Features.SetEntries.Domain;

public class SetEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Required]
    public DateOnly Date { get; init; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public double Result { get; init; }
    
    [Required]
    public int Reps { get; init; }
    
    [ForeignKey("ExerciseId")]
    public Exercise? Exercise { get; init; }
    
    public int ExerciseId { get; init; }
}
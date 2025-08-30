using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Session.SessionExercises.Entities;

namespace C_Sharp_Web_API.Session.SetEntries.Entities;

public class SetEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [Range(0, 9999.99)]
    [Column(TypeName = "decimal(6,2)")]
    public decimal Weight { get; set; }
    
    [Required]
    [Range(0, 100)]
    public int Reps { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int SetIndex { get; set; }
    
    [Required]
    public int SessionExerciseId { get; set; }

    public SessionExercise SessionExercise { get; set; } = null!;
}
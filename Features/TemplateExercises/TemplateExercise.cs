using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Features.SetEntries.Domain;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.Exercises.Domain;

public enum Unit { Kg, Min, Sec, Wdh }

[Index(nameof(Name), IsUnique = true)]
public class TemplateExercise(string name, Unit unit = Unit.Kg)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Required, MaxLength(100)]
    public string Name { get; init; } = name;
    
    [MaxLength(100)]
    public string? MuscleGroup { get; init; }
    
    [Required]
    public Unit Unit { get; init; } = unit;
}
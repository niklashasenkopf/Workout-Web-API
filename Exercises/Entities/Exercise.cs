using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Exercises.Entities;

public enum Unit { Kg, Lbs, Min, Sec }

[Index(nameof(Name), IsUnique = true)]
public class Exercise(string name, Unit unit = Unit.Kg)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = name;
    
    public string? MuscleGroup { get; set; }
    
    [Required]
    public Unit Unit { get; set; } = unit;
}
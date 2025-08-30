using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Template.WorkoutTemplates.Entities;

[Index(nameof(Name), IsUnique = true)]
public class WorkoutTemplate(string name)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required] 
    public string Name { get; set; } = name;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using C_Sharp_Web_API.Template.WorkoutTemplates.Entities;

namespace C_Sharp_Web_API.Session.WorkoutSessions.Entities;

public class WorkoutSession
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public DateTime StartedAt { get; set; } // Maps to TIMESTAMPTZ
    
    public DateTimeOffset? FinishesAt { get; set; }
    
    public int WorkoutTemplateId { get; set; }

    public WorkoutTemplate? WorkoutTemplate { get; set; } = null!;

}
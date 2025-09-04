using C_Sharp_Web_API.Authentication;
using C_Sharp_Web_API.Features.SetEntries;
using C_Sharp_Web_API.Features.TemplateExercises;
using C_Sharp_Web_API.Features.WorkoutExercises;
using C_Sharp_Web_API.Features.Workouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.DbContexts;

public class AppDatabaseContext : IdentityDbContext<ApiUser, IdentityRole<Guid>, Guid>
{
    public DbSet<TemplateExercise> TemplateExercises { get; set; }
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
    public DbSet<SetEntry> SetEntries { get; set; }
    public DbSet<Workout> Workouts { get; set; }

    public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TemplateExercise>().HasData(
            new TemplateExercise("Incline Bench Press") { Id = 1, MuscleGroup = "Chest", Unit = Unit.Kg},
            new TemplateExercise("Lat Pulldown") { Id = 2, MuscleGroup = "Back", Unit = Unit.Kg },
            new TemplateExercise("Horizontal Bench Press") { Id = 3, MuscleGroup = "Chest", Unit = Unit.Kg},
            new TemplateExercise("Chest Supported Row") { Id = 4, MuscleGroup = "Back", Unit = Unit.Kg},
            new TemplateExercise("Lateral Raise") { Id = 5, MuscleGroup = "Shoulders", Unit = Unit.Kg},
            new TemplateExercise("Triceps Extension") { Id = 6, MuscleGroup = "Triceps", Unit = Unit.Kg},
            new TemplateExercise("Preacher Curls") { Id = 7, MuscleGroup = "Biceps", Unit = Unit.Kg},
            new TemplateExercise("Weighted Leg Raises") { Id = 8, MuscleGroup = "Hip-Flexor", Unit = Unit.Kg},
            new TemplateExercise("Bulgarian Split-Squat") { Id = 9, MuscleGroup = "Quadriceps", Unit = Unit.Kg},
            new TemplateExercise("Copenhagen Plank") {Id = 10, MuscleGroup = "Thighs", Unit = Unit.Sec},
            new TemplateExercise("Hip Thrust") { Id = 11, MuscleGroup = "Glutes", Unit = Unit.Kg},
            new TemplateExercise("Calve Raises") { Id = 12, MuscleGroup = "Calves", Unit = Unit.Kg},
            new TemplateExercise("Crunch Machine") {Id = 13, MuscleGroup = "Abs", Unit = Unit.Kg},
            new TemplateExercise("Hanging Leg Raises") { Id = 14, MuscleGroup = "Abs", Unit = Unit.Kg}
        );
    }
}
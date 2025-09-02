using C_Sharp_Web_API.Authentication;
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.SetEntries.Domain;
using C_Sharp_Web_API.Features.Workouts.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.DbContexts;

public class AppDatabaseContext : IdentityDbContext<ApiUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<SetEntry> SetEntries { get; set; }
    public DbSet<Workout> Workouts { get; set; }

    public AppDatabaseContext(DbContextOptions<AppDatabaseContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Workout>().HasData(
            new Workout("Upper Body") { Id = 1},
            new Workout("Lower Body") { Id = 2}
        );

        modelBuilder.Entity<Exercise>().HasData(
            new Exercise("Incline Bench Press") { Id = 1, MuscleGroup = "Chest", Unit = Unit.Kg},
            new Exercise("Lat Pulldown") { Id = 2, MuscleGroup = "Back", Unit = Unit.Kg },
            new Exercise("Horizontal Bench Press") { Id = 3, MuscleGroup = "Chest", Unit = Unit.Kg},
            new Exercise("Chest Supported Row") { Id = 4, MuscleGroup = "Back", Unit = Unit.Kg},
            new Exercise("Lateral Raise") { Id = 5, MuscleGroup = "Shoulders", Unit = Unit.Kg},
            new Exercise("Triceps Extension") { Id = 6, MuscleGroup = "Triceps", Unit = Unit.Kg},
            new Exercise("Preacher Curls") { Id = 7, MuscleGroup = "Biceps", Unit = Unit.Kg},
            new Exercise("Weighted Leg Raises") { Id = 8, MuscleGroup = "Hip-Flexor", Unit = Unit.Kg},
            new Exercise("Bulgarian Split-Squat") { Id = 9, MuscleGroup = "Quadriceps", Unit = Unit.Kg},
            new Exercise("Copenhagen Plank") {Id = 10, MuscleGroup = "Thighs", Unit = Unit.Sec},
            new Exercise("Hip Thrust") { Id = 11, MuscleGroup = "Glutes", Unit = Unit.Kg},
            new Exercise("Calve Raises") { Id = 12, MuscleGroup = "Calves", Unit = Unit.Kg},
            new Exercise("Crunch Machine") {Id = 13, MuscleGroup = "Abs", Unit = Unit.Kg},
            new Exercise("Hanging Leg Raises") { Id = 14, MuscleGroup = "Abs", Unit = Unit.Kg}
        );

        modelBuilder.Entity<SetEntry>().HasData(
            new SetEntry() { Id = 1, ExerciseId = 1, Date = new DateOnly(2025, 8, 9), Result = 62.5, Reps = 7 },
            new SetEntry() { Id = 2, ExerciseId = 1, Date = new DateOnly(2025, 8, 9), Result = 62.5, Reps = 5 }
        );

        modelBuilder.Entity<Workout>()
            .HasMany(w => w.Exercises)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "WorkoutExercise",
                r => r.HasOne<Exercise>().WithMany().HasForeignKey("ExerciseId"),
                l => l.HasOne<Workout>().WithMany().HasForeignKey("WorkoutId"),
                je =>
                {
                    je.HasKey("WorkoutId", "ExerciseId");
                    je.ToTable("WorkoutExercise");

                    je.HasData(
                        new { WorkoutId = 1, ExerciseId = 1},
                        new { WorkoutId = 1, ExerciseId = 2},
                        new { WorkoutId = 1, ExerciseId = 3},
                        new { WorkoutId = 1, ExerciseId = 4},
                        new { WorkoutId = 1, ExerciseId = 5},
                        new { WorkoutId = 1, ExerciseId = 6},
                        new { WorkoutId = 1, ExerciseId = 7}
                    );
                }
            );
    }
}
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.Workouts.Domain;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public interface IWorkoutRepository
{
    // Basic CRUD operations
    Task<IEnumerable<Workout>> GetAllAsync();
    Task<Workout?> GetAsync(int workoutId, bool includeExercises = false);
    Task CreateAsync(Workout workout);
    void Delete(Workout workout); 
    
    // WorkoutExercises Join Table
    void AddExerciseToWorkout(Workout workout, Exercise exercise);
    void RemoveExerciseFromWorkout(Workout workout, Exercise exercise); 
    
    // Helper methods
    Task<bool> WorkoutExistsAsync(int workoutId);
    Task<int> SaveChangesAsync(); 
}
using C_Sharp_Web_API.Features.Workouts.Domain;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public interface IWorkoutRepository
{
    // Basic CRUD operations
    Task<IEnumerable<Workout>> GetAllAsync();
    Task<Workout?> GetAsync(int workoutId, bool includeExercises = false); 
    
    // Helper methods
    Task<bool> WorkoutExistsAsync(int workoutId); 
    
}
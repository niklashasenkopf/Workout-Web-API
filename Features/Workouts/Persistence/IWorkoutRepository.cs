using C_Sharp_Web_API.Features.SetEntries.Domain;
using C_Sharp_Web_API.FeaturesNew.WorkoutExercises.Domain;
using C_Sharp_Web_API.Shared;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public interface IWorkoutRepository
{
    // Workout Operations
    Task<(IEnumerable<Workout>, PaginationMetadata)> GetAllAsync(
        string? name, string? searchQuery, int pageNumber, int pageSize);
    Task<Workout?> GetAsync(int workoutId);
    Task CreateAsync(Workout workout);
    void Delete(Workout workout); 
    Task<bool> ExistsAsync(int workoutId);
    
    // WorkoutExercise Operations
    Task<IEnumerable<WorkoutExercise>> GetAllExercisesAsync(
        int workoutId);
    Task<WorkoutExercise?> GetExerciseAsync(
        int workoutId, int workoutExerciseId);
    Task CreateExerciseAsync(
        int workoutId, WorkoutExercise workoutExercise);
    void DeleteExercise(
        int workoutId, WorkoutExercise workoutExercise);
    
    // SetEntry Operations
    Task<(IEnumerable<SetEntry>, PaginationMetadata)> GetAllSetEntriesAsync(
        int workoutId, int workoutExerciseId, DateOnly? date, string? searchQuery, int pageSize, int pageNumber);
    Task<SetEntry?> GetSetEntryAsync(
        int workoutId, int workoutExerciseId, int setEntryId);
    Task CreateSetEntryAsync(
        int workoutId, int workoutExerciseId, SetEntry setEntry);
    void DeleteSetEntry(
       int workoutId, int workoutExerciseId, SetEntry setEntry);
    
    // Helper methods
    Task<int> SaveChangesAsync(); 
}
using C_Sharp_Web_API.Features.SetEntries;
using C_Sharp_Web_API.Features.WorkoutExercises;
using C_Sharp_Web_API.Shared;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public interface IWorkoutRepository
{
    // Workout Operations
    Task<(IEnumerable<Workout>, PaginationMetadata)> GetAllAsync(
        Guid apiUserId, string? name, string? searchQuery, int pageNumber, int pageSize);
    Task<Workout?> GetAsync(Guid apiUserId, int workoutId);
    Task CreateAsync(Workout workout);
    void Delete(Workout workout); 
    Task<bool> WorkoutExistsAsync(Guid apiUserId, int workoutId);
    
    // WorkoutExercise Operations
    Task<IEnumerable<WorkoutExercise>> GetAllExercisesAsync(
        Guid apiUserId, int workoutId);
    Task<WorkoutExercise?> GetExerciseAsync(
        Guid apiUserId, int workoutId, int workoutExerciseId);
    Task CreateExerciseAsync(WorkoutExercise workoutExercise);
    void DeleteExercise(WorkoutExercise workoutExercise);
    Task<bool> WorkoutExerciseExistsAsync(Guid apiUserId, int workoutId, int workoutExerciseId);
    
    // SetEntry Operations
    Task<(IEnumerable<SetEntry>, PaginationMetadata)> GetAllSetEntriesAsync(
        Guid apiUserId, int workoutId, int workoutExerciseId, DateOnly? date, string? searchQuery, int pageSize, int pageNumber);
    Task<SetEntry?> GetSetEntryAsync(
        Guid apiUserId, int workoutId, int workoutExerciseId, int setEntryId);
    Task CreateSetEntryAsync(SetEntry setEntry);
    void DeleteSetEntry(SetEntry setEntry);
    
    // Helper methods
    Task<int> SaveChangesAsync(); 
}
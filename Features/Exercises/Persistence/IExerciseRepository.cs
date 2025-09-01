using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.SetEntries.Domain;

namespace C_Sharp_Web_API.Features.Exercises.Persistence;

public interface IExerciseRepository
{
    // Methods for Exercise Entity
    Task<IEnumerable<Exercise>> GetExercisesAsync();
    Task<Exercise?> GetExerciseAsync(int exerciseId, bool includeSetEntries = false);
    Task CreateExerciseAsync(Exercise exercise);
    void DeleteExercise(Exercise exercise);
    
    // Methods for nested Set Entries for Exercise
    Task<IEnumerable<SetEntry>> GetSetEntriesForExercise(int exerciseId);
    Task<SetEntry?> GetSetEntryForExercise(int exerciseId, int setEntryId);
    Task AddSetEntryForExerciseAsync(int exerciseId, SetEntry setEntry);
    void DeleteSetEntryForExercise(SetEntry setEntry);
    
    // Helper Methods
    Task<bool> SaveChangesAsync();
    Task<bool> ExerciseExistsAsync(int exerciseId);
    
}
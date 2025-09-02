using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.SetEntries.Domain;
using C_Sharp_Web_API.Shared;

namespace C_Sharp_Web_API.Features.Exercises.Persistence;

public interface IExerciseRepository
{
    // Methods for Exercise Entity
    Task<Exercise?> GetAsync(int exerciseId, bool includeSetEntries = false);
    Task CreateAsync(Exercise exercise);
    void Delete(Exercise exercise);
    
    // Methods for nested Set Entries for Exercise
    Task<(IEnumerable<SetEntry>, PaginationMetadata)> GetSetEntriesForExercise(
        int exerciseId, DateOnly? date, string? searchQuery, int pageSize, int pageNumber );
    Task<SetEntry?> GetSetEntryForExercise(int exerciseId, int setEntryId);
    Task AddSetEntryForExerciseAsync(int exerciseId, SetEntry setEntry);
    void DeleteSetEntryForExercise(SetEntry setEntry);
    
    // Filtering & Searching
    Task<(IEnumerable<Exercise>, PaginationMetadata)> GetAllAsync(
        string? name, 
        string? searchQuery, 
        int pageNumber, 
        int pageSize);
    
    // Helper Methods
    Task<bool> SaveChangesAsync();
    Task<bool> ExistsAsync(int exerciseId);
    
}
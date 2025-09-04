using C_Sharp_Web_API.Features.TemplateExercises;
using C_Sharp_Web_API.Shared;

namespace C_Sharp_Web_API.Features.TemplateExercises.Persistence;

public interface ITemplateExerciseRepository
{
    // Methods for Exercise Entity
    Task<(IEnumerable<TemplateExercise>, PaginationMetadata)> GetAllAsync(
        string? name, 
        string? searchQuery, 
        int pageNumber, 
        int pageSize);
    Task<TemplateExercise?> GetAsync(int exerciseId, bool includeSetEntries = false);
    Task CreateAsync(TemplateExercise templateExercise);
    void Delete(TemplateExercise templateExercise);
    
    // Helper Methods
    Task<bool> SaveChangesAsync();
    Task<bool> ExistsAsync(int exerciseId);
    
}
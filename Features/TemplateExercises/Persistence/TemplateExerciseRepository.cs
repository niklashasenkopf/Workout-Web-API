using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.SetEntries;
using C_Sharp_Web_API.Shared;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.TemplateExercises.Persistence;

public class TemplateExerciseRepository(AppDatabaseContext context) : ITemplateExerciseRepository
{

    private readonly AppDatabaseContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<(IEnumerable<TemplateExercise>, PaginationMetadata)> GetAllAsync(
        string? name, 
        string? searchQuery, 
        int pageNumber, 
        int pageSize)
    { 
        // collection to start from
        IQueryable<TemplateExercise> collection = _context.TemplateExercises;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(c => c.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(e => e.Name.Contains(searchQuery));
        }

        var totalItemCount = await collection.CountAsync();

        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

        var collectionToReturn =  await collection
            .OrderBy(e => e.Name)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetadata);
    }

    public async Task<TemplateExercise?> GetAsync(int exerciseId, bool includeSetEntries = false)
    {
        return await _context.TemplateExercises.Where(e => e.Id == exerciseId).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(TemplateExercise templateExercise)
    {
        await _context.TemplateExercises.AddAsync(templateExercise);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 0); 
    }

    public void Delete(TemplateExercise templateExercise)
    {
        _context.TemplateExercises.Remove(templateExercise);
    }

    public async Task<SetEntry?> GetSetEntryForExercise(int exerciseId, int setEntryId)
    {
        return await _context.SetEntries
            .Where(se => se.WorkoutExerciseId == exerciseId && se.Id == setEntryId)
            .FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<SetEntry>, PaginationMetadata)> GetSetEntriesForExercise(
        int exerciseId,
        DateOnly? date,
        string? searchQuery,
        int pageSize,
        int pageNumber)
    {
        var collection = _context.SetEntries.Where(se => se.WorkoutExerciseId == exerciseId);

        if (date.HasValue)
        {
            collection = collection.Where(se => se.Date == date);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            if (DateOnly.TryParseExact(searchQuery, "yyyy-MM-dd", out var exactDate))
            {
                collection = collection.Where(se => se.Date == exactDate);
            }
            else if(DateOnly.TryParseExact(searchQuery, "yyyy-MM", out var yearMonth))
            {
                collection = collection.Where(
                    se => se.Date.Year == yearMonth.Year && se.Date.Month == yearMonth.Month);
            }
            else if (int.TryParse(searchQuery, out var year))
            {
                collection = collection.Where(se => se.Date.Year == year);
            }
        }

        var totalItemCount = await collection.CountAsync();

        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
        
        
        var collectionToReturn =  await collection
            .OrderByDescending(se => se.Date)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetadata);
    }

    public async Task<bool> ExistsAsync(int exerciseId)
    {
        return await _context.TemplateExercises.AnyAsync(e => e.Id == exerciseId);
    }
}
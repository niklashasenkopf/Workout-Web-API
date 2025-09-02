using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.SetEntries.Domain;
using C_Sharp_Web_API.Shared;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.Exercises.Persistence;

public class ExerciseRepository(AppDatabaseContext context) : IExerciseRepository
{

    private readonly AppDatabaseContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<(IEnumerable<Exercise>, PaginationMetadata)> GetAllAsync(
        string? name, 
        string? searchQuery, 
        int pageNumber, 
        int pageSize)
    { 
        // collection to start from
        IQueryable<Exercise> collection = _context.Exercises;

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

    public async Task<Exercise?> GetAsync(int exerciseId, bool includeSetEntries = false)
    {
        if (includeSetEntries)
        {
            return await _context.Exercises.Include(e => e.SetEntries)
                .Where(e => e.Id == exerciseId).FirstOrDefaultAsync();
        }

        return await _context.Exercises.Where(e => e.Id == exerciseId).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Exercise exercise)
    {
        await _context.Exercises.AddAsync(exercise);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 0); 
    }

    public void Delete(Exercise exercise)
    {
        _context.Exercises.Remove(exercise);
    }

    public async Task<SetEntry?> GetSetEntryForExercise(int exerciseId, int setEntryId)
    {
        return await _context.SetEntries
            .Where(se => se.ExerciseId == exerciseId && se.Id == setEntryId)
            .FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<SetEntry>, PaginationMetadata)> GetSetEntriesForExercise(
        int exerciseId,
        DateOnly? date,
        string? searchQuery,
        int pageSize,
        int pageNumber)
    {
        var collection = _context.SetEntries.Where(se => se.ExerciseId == exerciseId);

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
        return await _context.Exercises.AnyAsync(e => e.Id == exerciseId);
    }

    public async Task AddSetEntryForExerciseAsync(int exerciseId, SetEntry setEntry)
    {
        var exercise = await GetAsync(exerciseId);
        
        exercise?.SetEntries.Add(setEntry);
    }

    public void DeleteSetEntryForExercise(SetEntry setEntry)
    {
        _context.SetEntries.Remove(setEntry);
    }
}
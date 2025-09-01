using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.SetEntries.Domain;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.Exercises.Persistence;

public class ExerciseRepository(WorkoutContext context) : IExerciseRepository
{

    private readonly WorkoutContext _context = context ?? throw new ArgumentNullException(nameof(context));
    
    public async Task<IEnumerable<Exercise>> GetExercisesAsync()
    {
        return await _context.Exercises.ToListAsync();
    }

    public async Task<Exercise?> GetExerciseAsync(int exerciseId, bool includeSetEntries = false)
    {
        if (includeSetEntries)
        {
            return await _context.Exercises.Include(e => e.SetEntries)
                .Where(e => e.Id == exerciseId).FirstOrDefaultAsync();
        }

        return await _context.Exercises.Where(e => e.Id == exerciseId).FirstOrDefaultAsync();
    }

    public async Task CreateExerciseAsync(Exercise exercise)
    {
        await _context.Exercises.AddAsync(exercise);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync() >= 0); 
    }

    public void DeleteExercise(Exercise exercise)
    {
        _context.Exercises.Remove(exercise);
    }

    public async Task<SetEntry?> GetSetEntryForExercise(int exerciseId, int setEntryId)
    {
        return await _context.SetEntries
            .Where(se => se.ExerciseId == exerciseId && se.Id == setEntryId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SetEntry>> GetSetEntriesForExercise(int exerciseId)
    {
        return await _context.SetEntries.Where(se => se.ExerciseId == exerciseId).ToListAsync();
    }

    public async Task<bool> ExerciseExistsAsync(int exerciseId)
    {
        return await _context.Exercises.AnyAsync(e => e.Id == exerciseId);
    }

    public async Task AddSetEntryForExerciseAsync(int exerciseId, SetEntry setEntry)
    {
        var exercise = await GetExerciseAsync(exerciseId);
        
        exercise?.SetEntries.Add(setEntry);
    }

    public void DeleteSetEntryForExercise(SetEntry setEntry)
    {
        _context.SetEntries.Remove(setEntry);
    }
}
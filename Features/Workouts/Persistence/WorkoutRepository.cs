using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.SetEntries;
using C_Sharp_Web_API.Features.WorkoutExercises;
using C_Sharp_Web_API.Shared;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public class WorkoutRepository(AppDatabaseContext appDatabaseContext) : IWorkoutRepository
{
    private readonly AppDatabaseContext _appDatabaseContext =
        appDatabaseContext ?? throw new ArgumentNullException(nameof(appDatabaseContext));

    public async Task<(IEnumerable<Workout>, PaginationMetadata)> GetAllAsync(
        Guid apiUserId,
        string? name, 
        string? searchQuery, 
        int pageNumber, 
        int pageSize)
    {
        var collection = 
            _appDatabaseContext.Workouts.Where(w => w.ApiUserId == apiUserId);

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(w => w.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(w => w.Name.Contains(searchQuery));
        }

        var totalItemCount = await collection.CountAsync();

        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

        var collectionToReturn = await collection
            .OrderBy(w => w.Name)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetadata);
    }

    public async Task<Workout?> GetAsync(Guid apiUserId, int workoutId)
    {
        return await _appDatabaseContext.Workouts
            .Where(w => w.ApiUserId == apiUserId)
            .FirstOrDefaultAsync(w => w.Id == workoutId);
    }

    public async Task CreateAsync(Workout workout)
    {
        await _appDatabaseContext.Workouts.AddAsync(workout);
    }
    
    public void Delete(Workout workout)
    {
        _appDatabaseContext.Workouts.Remove(workout);
    }

    public async Task<bool> WorkoutExistsAsync(Guid apiUserId, int workoutId)
    {
        return await _appDatabaseContext.Workouts
            .Where(w => w.ApiUserId == apiUserId)
            .AnyAsync(w => w.Id == workoutId);
    }

    public async Task<IEnumerable<WorkoutExercise>> GetAllExercisesAsync(Guid apiUserId, int workoutId)
    {
        var collection = await _appDatabaseContext.WorkoutExercises
            .Where(we => we.WorkoutId == workoutId 
                         && we.Workout.ApiUserId == apiUserId)
            .Include(we => we.TemplateExercise)
            .ToListAsync();

        return collection;
    }

    public async Task<WorkoutExercise?> GetExerciseAsync(Guid apiUserId, int workoutId, int workoutExerciseId)
    {
        var exercise = await _appDatabaseContext.WorkoutExercises
            .Where(we => we.Id == workoutExerciseId 
                         && we.WorkoutId == workoutId 
                         && we.Workout.ApiUserId == apiUserId)
            .FirstOrDefaultAsync();

        return exercise;
    }

    public async Task CreateExerciseAsync(WorkoutExercise workoutExercise)
    {
        await _appDatabaseContext.WorkoutExercises.AddAsync(workoutExercise);
    }

    public void DeleteExercise(WorkoutExercise workoutExercise)
    {
        _appDatabaseContext.WorkoutExercises.Remove(workoutExercise);
    }

    public async Task<bool> WorkoutExerciseExistsAsync(Guid apiUserId, int workoutId, int workoutExerciseId)
    {
        return await _appDatabaseContext.WorkoutExercises
            .Where(we => we.Id == workoutExerciseId && we.WorkoutId == workoutId && we.Workout.ApiUserId == apiUserId)
            .AnyAsync();
    }

    public async Task<(IEnumerable<SetEntry>, PaginationMetadata)> GetAllSetEntriesAsync(
        Guid apiUserId, int workoutId, int workoutExerciseId, 
        DateOnly? date, string? searchQuery, int pageSize, int pageNumber)
    {
        var collection = _appDatabaseContext.SetEntries
            .Where(se => se.WorkoutExerciseId == workoutExerciseId
                         && se.WorkoutExercise.WorkoutId == workoutId
                         && se.WorkoutExercise.Workout.ApiUserId == apiUserId);

        if (date.HasValue)
        {
            collection = collection.Where(se => se.Date == date);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(se => se.Date.ToString().Contains(searchQuery));
        }

        var totalItemCount = await collection.CountAsync();

        var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);
        
        var collectionToReturn = await collection
            .OrderByDescending(se => se.Date)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetadata);
    }

    public async Task<SetEntry?> GetSetEntryAsync(
        Guid apiUserId, int workoutId, int workoutExerciseId, int setEntryId)
    {
        return await _appDatabaseContext.SetEntries
            .Where(se => se.Id == setEntryId
                         && se.WorkoutExerciseId == workoutExerciseId
                         && se.WorkoutExercise.WorkoutId == workoutId
                         && se.WorkoutExercise.Workout.ApiUserId == apiUserId
            )
            .FirstOrDefaultAsync();
    }

    public async Task CreateSetEntryAsync(SetEntry setEntry)
    {
         await _appDatabaseContext.SetEntries.AddAsync(setEntry);
    }

    public void DeleteSetEntry(SetEntry setEntry)
    {
        _appDatabaseContext.SetEntries.Remove(setEntry);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _appDatabaseContext.SaveChangesAsync();
    }
}
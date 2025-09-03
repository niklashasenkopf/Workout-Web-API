using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.SetEntries.Domain;
using C_Sharp_Web_API.FeaturesNew.WorkoutExercises.Domain;
using C_Sharp_Web_API.Shared;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public class WorkoutRepository(AppDatabaseContext appDatabaseContext) : IWorkoutRepository
{
    private readonly AppDatabaseContext _appDatabaseContext =
        appDatabaseContext ?? throw new ArgumentNullException(nameof(appDatabaseContext));

    public async Task<(IEnumerable<Workout>, PaginationMetadata)> GetAllAsync(
        string? name, 
        string? searchQuery, 
        int pageNumber, 
        int pageSize)
    {
        IQueryable<Workout> collection = _appDatabaseContext.Workouts;

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

    public async Task<Workout?> GetAsync(int workoutId)
    {
        return await _appDatabaseContext.Workouts.FirstOrDefaultAsync(w => w.Id == workoutId);
    }

    public async Task CreateAsync(Workout workout)
    {
        await _appDatabaseContext.Workouts.AddAsync(workout);
    }
    
    public void Delete(Workout workout)
    {
        _appDatabaseContext.Workouts.Remove(workout);
    }

    public async Task<bool> ExistsAsync(int workoutId)
    {
        return await _appDatabaseContext.Workouts.AnyAsync(w => w.Id == workoutId);
    }

    public Task<IEnumerable<WorkoutExercise>> GetAllExercisesAsync(int workoutId)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public Task<WorkoutExercise?> GetExerciseAsync(int workoutId, int workoutExerciseId)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public Task CreateExerciseAsync(int workoutId, WorkoutExercise workoutExercise)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public void DeleteExercise(int workoutId, WorkoutExercise workoutExercise)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public Task<(IEnumerable<SetEntry>, PaginationMetadata)> GetAllSetEntriesAsync(
        int workoutId, int workoutExerciseId, DateOnly? date, string? searchQuery, int pageSize, int pageNumber)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public Task<SetEntry?> GetSetEntryAsync(
        int workoutId, int workoutExerciseId, int setEntryId)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public Task CreateSetEntryAsync(
        int workoutId, int workoutExerciseId, SetEntry setEntry)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public void DeleteSetEntry(
        int workoutId, int workoutExerciseId, SetEntry setEntry)
    {
        //TODO: Implement
        throw new NotImplementedException();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _appDatabaseContext.SaveChangesAsync();
    }
}
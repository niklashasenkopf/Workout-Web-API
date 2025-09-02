using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.Workouts.Domain;
using C_Sharp_Web_API.Shared;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public class WorkoutRepository(WorkoutContext workoutContext) : IWorkoutRepository
{
    private readonly WorkoutContext _workoutContext =
        workoutContext ?? throw new ArgumentNullException(nameof(workoutContext));

    public async Task<(IEnumerable<Workout>, PaginationMetadata)> GetAllAsync(
        string? name, 
        string? searchQuery, 
        int pageNumber, 
        int pageSize)
    {
        IQueryable<Workout> collection = _workoutContext.Workouts;

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

    public async Task<Workout?> GetAsync(int workoutId, bool includeExercises)
    {
        if (includeExercises)
        {
            return await _workoutContext.Workouts
                .Include(w => w.Exercises)
                .FirstOrDefaultAsync(w => w.Id == workoutId);
        }

        return await _workoutContext.Workouts.FirstOrDefaultAsync(w => w.Id == workoutId);
    }

    public async Task CreateAsync(Workout workout)
    {
        await _workoutContext.Workouts.AddAsync(workout);
    }
    
    public void Delete(Workout workout)
    {
        _workoutContext.Workouts.Remove(workout);
    }

    public void AddExerciseToWorkout(Workout workout, Exercise exercise)
    {
        if (!workout.Exercises.Any(e => e.Id == exercise.Id))
        {
            workout.Exercises.Add(exercise);
        }
    }

    public void RemoveExerciseFromWorkout(Workout workout, Exercise exercise)
    {
        workout.Exercises.Remove(exercise);
    }

    public async Task<bool> WorkoutExistsAsync(int workoutId)
    {
        return await _workoutContext.Workouts.AnyAsync(w => w.Id == workoutId);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _workoutContext.SaveChangesAsync();
    }
}
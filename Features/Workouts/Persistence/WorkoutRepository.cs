using C_Sharp_Web_API.DbContexts;
using C_Sharp_Web_API.Features.Workouts.Domain;
using Microsoft.EntityFrameworkCore;

namespace C_Sharp_Web_API.Features.Workouts.Persistence;

public class WorkoutRepository(WorkoutContext workoutContext) : IWorkoutRepository
{
    private readonly WorkoutContext _workoutContext =
        workoutContext ?? throw new ArgumentNullException(nameof(workoutContext));

    public async Task<IEnumerable<Workout>> GetAllAsync()
    {
        return await _workoutContext.Workouts.ToListAsync();
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

    public async Task<bool> WorkoutExistsAsync(int workoutId)
    {
        return await _workoutContext.Workouts.AnyAsync(w => w.Id == workoutId);
    }
}
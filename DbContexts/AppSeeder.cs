using C_Sharp_Web_API.Authentication;
using C_Sharp_Web_API.Features.SetEntries;
using C_Sharp_Web_API.Features.WorkoutExercises;
using C_Sharp_Web_API.Features.Workouts;

namespace C_Sharp_Web_API.DbContexts;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class AppSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApiUser>>();

        // Ensure DB created / migrated
        await context.Database.MigrateAsync();

        // Seed user
        const string email = "test@example.com";
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApiUser
            {
                UserName = "testuser",
                Email = email,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, "Pass123!");
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        
        // Create second user
        const string email2 = "test2@example.com";
        var user2 = await userManager.FindByEmailAsync(email2);
        if (user2 == null)
        {
            user2 = new ApiUser
            {
                UserName = "testuser2",
                Email = email2,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user2, "Pass123!");
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Seed workouts if not already present
        if (!context.Workouts.Any())
        {
            var upperBody = new Workout("Upper Body") { ApiUserId = user.Id };
            var lowerBody = new Workout("Lower Body") { ApiUserId = user.Id };

            context.Workouts.AddRange(upperBody, lowerBody);
            await context.SaveChangesAsync();

            // Attach workout exercises
            var benchPress = new WorkoutExercise
            {
                Order = 1,
                TemplateExerciseId = 1, // Incline Bench Press
                SetEntries =
                {
                    new SetEntry { Date = new DateOnly(2025, 9, 1), Result = 60, Reps = 8 },
                    new SetEntry { Date = new DateOnly(2025, 9, 1), Result = 60, Reps = 6 }
                }
            };
            var latPulldown = new WorkoutExercise
            {
                Order = 2,
                TemplateExerciseId = 2, // Lat Pulldown
                SetEntries =
                {
                    new SetEntry { Date = new DateOnly(2025, 9, 1), Result = 50, Reps = 10 }
                }
            };

            upperBody.WorkoutExercises.Add(benchPress);
            upperBody.WorkoutExercises.Add(latPulldown);

            var squat = new WorkoutExercise
            {
                Order = 1,
                TemplateExerciseId = 9, // Bulgarian Split-Squat
                SetEntries =
                {
                    new SetEntry { Date = new DateOnly(2025, 9, 1), Result = 40, Reps = 12 }
                }
            };

            lowerBody.WorkoutExercises.Add(squat);

            await context.SaveChangesAsync();
        }
    }
}

using C_Sharp_Web_API.Features.Workouts.Domain;
using Microsoft.AspNetCore.Identity;

namespace C_Sharp_Web_API.Authentication;

public class ApiUser : IdentityUser<Guid>
{
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
using AutoMapper;
using C_Sharp_Web_API.Features.Workouts.Domain;
using C_Sharp_Web_API.Features.Workouts.Dtos;

namespace C_Sharp_Web_API.Features.Workouts.Mappings;

public class WorkoutsMappingProfile : Profile
{
    public WorkoutsMappingProfile()
    {
        CreateMap<Workout, WorkoutDto>();
        CreateMap<WorkoutDto, Workout>();
        CreateMap<Workout, WorkoutWithoutExercisesDto>();
        CreateMap<WorkoutWithoutExercisesDto, WorkoutDto>();
    }
}
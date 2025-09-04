using AutoMapper;
using C_Sharp_Web_API.Features.WorkoutExercises.Dtos;

namespace C_Sharp_Web_API.Features.WorkoutExercises;

public class WorkoutExerciseMappingsProfile : Profile
{
    public WorkoutExerciseMappingsProfile()
    {
        CreateMap<WorkoutExerciseDto, WorkoutExercise>();
        CreateMap<WorkoutExercise, WorkoutExerciseDto>();
    }
}
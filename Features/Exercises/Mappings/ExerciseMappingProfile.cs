using AutoMapper;
using C_Sharp_Web_API.Features.Exercises.Domain;
using C_Sharp_Web_API.Features.Exercises.dtos;
using C_Sharp_Web_API.Features.Exercises.Dtos;

namespace C_Sharp_Web_API.Features.Exercises.Mappings;

public class ExerciseMappingProfile : Profile
{
    public ExerciseMappingProfile()
    {
        CreateMap<Exercise, ExerciseDto>();
        CreateMap<ExerciseDto, Exercise>();
        CreateMap<Exercise, ExerciseWithoutSetEntriesDto>();
        CreateMap<ExerciseCreateRequestDto, Exercise>();
        CreateMap<ExerciseUpdateRequestDto, Exercise>();
        CreateMap<Exercise, ExerciseUpdateRequestDto>();
    }
}
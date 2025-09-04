using AutoMapper;
using C_Sharp_Web_API.Features.TemplateExercises.Dtos;

namespace C_Sharp_Web_API.Features.TemplateExercises;

public class TemplateExerciseMappingProfile : Profile
{
    public TemplateExerciseMappingProfile()
    {
        CreateMap<TemplateExercise, TemplateExerciseDto>();
        CreateMap<TemplateExerciseDto, TemplateExercise>();
        CreateMap<TemplateExerciseCreateRequestDto, TemplateExercise>();
        CreateMap<TemplateExerciseUpdateRequestDto, TemplateExercise>();
        CreateMap<TemplateExercise, TemplateExerciseUpdateRequestDto>();
    }
}
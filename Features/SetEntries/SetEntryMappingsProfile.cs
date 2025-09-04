using AutoMapper;
using C_Sharp_Web_API.Features.SetEntries.Dtos;

namespace C_Sharp_Web_API.Features.SetEntries;

public class SetEntryMappingsProfile : Profile
{
    public SetEntryMappingsProfile()
    {
        CreateMap<SetEntry, SetEntryDto>();
        CreateMap<SetEntryCreateRequestDto, SetEntry>();
        CreateMap<SetEntryUpdateRequestDto, SetEntry>();
        CreateMap<SetEntry, SetEntryUpdateRequestDto>();
    }
}
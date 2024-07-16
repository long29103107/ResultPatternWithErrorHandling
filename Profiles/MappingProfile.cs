using AutoMapper;
using FluentValidation.Results;
using ResultPatternExample.Exceptions;

namespace ResultPatternExample.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ValidationFailure, Error>()
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.ErrorMessage));
    }
}

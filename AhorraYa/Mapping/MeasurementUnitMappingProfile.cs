using AhorraYa.Application.Dtos.MeasurementUnit;
using AhorraYa.Entities;
using AutoMapper;

namespace AhorraYa.WebApi.Mapping
{
    public class MeasurementUnitMappingProfile : Profile
    {
        public MeasurementUnitMappingProfile()
        {
            CreateMap<MeasurementUnit, MeasurementUnitResponseDto>();
            CreateMap<MeasurementUnitRequestDto, MeasurementUnit>().ReverseMap();
        }
    }
}
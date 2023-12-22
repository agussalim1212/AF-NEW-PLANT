using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SkeletonApi.Domain.Entities;
using SkeletonApi.IotHub.DTOs;
using SkeletonApi.IotHub.Model;
using System;
using System.Globalization;
using System.Reflection;

namespace SkeletonApi.IotHub;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

        //CreateMap<EnginePartDto, EnginePart>();
        //CreateMap<MainSubject, MainSubjectListDto>();
        //CreateMap<MainSubjectListDto, MainSubject>();
        //CreateMap<MainSubject, MainSubject>();

        //CreateMap<MqttRawValue, MqttRawValueEntity>()
        //    .ForMember(c => c.Datetime, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.Time).DateTime));
        //CreateMap<Subject, SubjectStoreDto>();
        //CreateMap<MainSubject, MainSubjectStoreDto>();
        //CreateMap<Subject, SubjectStoreDto>().ReverseMap();
        //CreateMap<TraceabilityResult, TraceabilityResultDto>();
        //CreateMap<TraceabilityResult, TraceabilityResultDto>().ReverseMap();

        // CreateMap<ConsumptionDataResult, ConsumptionDataResultDto>().ReverseMap();

        CreateMap<MachineStatusDto, Machine>();
        CreateMap<Machine, MachineStatusDto>();
        CreateMap<Machine, MachineStatusDto>().ReverseMap();

    }
}




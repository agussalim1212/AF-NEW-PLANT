using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Domain.Entities.Tsdb;
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
        CreateMap<EnginePartDto, EnginePart>().ReverseMap();
        CreateMap<MqttRawValue, MqttRawValueEntity>()
           .ForMember(c => c.Datetime, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.Time).DateTime));
        CreateMap<MachineStatusDto, Machine>();
        CreateMap<Machine, MachineStatusDto>();
        CreateMap<Machine, MachineStatusDto>().ReverseMap();

        CreateMap<Setting, NotificationDto>().ReverseMap();
        CreateMap<Subject, SubjectDto>().ReverseMap();
        CreateMap<NotificationModel, Notifications>().ReverseMap();
    }
}
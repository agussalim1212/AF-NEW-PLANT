using AutoMapper;
using SkeletonApi.Application.Features.Machines.Commands.CreateMachines;
using SkeletonApi.Application.Features.Machines;
using System.Reflection;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Application.Features.Subjects.Commands.CreateSubject;
using SkeletonApi.Application.Features.Subjects;
using SkeletonApi.Application.Features.FrameNumb.Commands.CreateFrameNumber;
using SkeletonApi.Application.Features.FrameNumb;
using SkeletonApi.Application.Features.Users;
using SkeletonApi.Application.Features.ManagementUser.Users.Commands.CreateUser;
using SkeletonApi.Application.Features.ManagementUser.Roles.Commands.CreateRoles;
using SkeletonApi.Application.Features.ManagementUser.Roles;
using SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.CreatePermissions;
using SkeletonApi.Application.Features.ManagementUser.Permissions;
using SkeletonApi.Application.Features.ManagementUser.Permissions.Commands.UpdatePermissions;
using SkeletonApi.Application.Features.Accounts;
using SkeletonApi.Application.Features.Accounts.Profiles.Commands.CreateAccount;
using SkeletonApi.Application.Features.Settings.Commands.CreateSetting;
using SkeletonApi.Application.Features.Settings;
using SkeletonApi.Domain.Entities.Tsdb;
using SkeletonApi.Application.Features.Dashboard.Traceability_History.Queries;
using SkeletonApi.Application.Features.MaintenancesPreventive.Commands.UploadExcel;

namespace SkeletonApi.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

            CreateMap<CreateMachineRequest, Machine>();
            CreateMap<Machine, CreateMachineResponseDto>();

            CreateMap<CreateSubjectRequest, Subject>();
            CreateMap<Subject, CreateSubjectResponseDto>();

            CreateMap<CreateFrameNumberRequest, FrameNumber>();
            CreateMap<FrameNumber, CreateFrameNumberResponseDto>(); 
            
            CreateMap<CreateUserRequest, User>();
            CreateMap<User, CreateUserResponseDto>();

            CreateMap<CreateRolesRequest, Role>();
            CreateMap<Role, CreateRolesResponseDto>();

            CreateMap<CreatePermissionsRequest, Permission>();
            CreateMap<Permission, CreatePermissionsResponseDto>();

            CreateMap<UpdatePermissionsRequest, Permission>();

            CreateMap<CreateAccountRequest, Account>();
            CreateMap<Account, CreateAccountResponseDto>();

            CreateMap<CreateSettingRequest, Setting>();
            CreateMap<Setting, CreateSettingResponseDto>();

            CreateMap<MaintenacePreventive, UploadExcelMaintPrevDto>().ReverseMap();

            //CreateMap<EnginePart, GetAllTraceabilityHistoryDto>();

        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var mapFromType = typeof(IMapFrom<>);

            var mappingMethodName = nameof(IMapFrom<object>.Mapping);

            bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

            var types = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Any(HasInterface)).ToList();

            var argumentTypes = new Type[] { typeof(Profile) };

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod(mappingMethodName);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(instance, new object[] { this });
                }
                else
                {
                    var interfaces = type.GetInterfaces().Where(HasInterface).ToList();

                    if (interfaces.Count > 0)
                    {
                        foreach (var @interface in interfaces)
                        {
                            var interfaceMethodInfo = @interface.GetMethod(mappingMethodName, argumentTypes);

                            interfaceMethodInfo.Invoke(instance, new object[] { this });
                        }
                    }
                }
            }
        }
    }
}

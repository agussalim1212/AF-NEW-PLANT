using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Persistence.Repositories;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;


namespace SkeletonApi.Application.Features.Settings.Commands.CreateSetting
{
    internal class CreateSettingCommandHandler : IRequestHandler<CreateSettingRequest, Result<CreateSettingResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;

        public CreateSettingCommandHandler(IUnitOfWork unitOfWork, ISettingRepository settingRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = settingRepository;
            _mapper = mapper;
        }

        public async Task<Result<CreateSettingResponseDto>> Handle(CreateSettingRequest request, CancellationToken cancellationToken)
        {
            var settings = _mapper.Map<Setting>(request);
            // var cekSetting = await _settingRepository.ValidateSetting(settings);
            var cekSetting = await _unitOfWork.Repository<Setting>().FindByCondition(a => a.MachineName.ToLower() == request.Name.ToLower() && a.SubjectName.ToLower() == request.Subject.ToLower()).CountAsync();
            if (cekSetting > 0)
            {
                var cek = _unitOfWork.Repository<Setting>().FindByCondition(a => a.MachineName.ToLower() == request.Name.ToLower() && a.SubjectName.ToLower() == request.Subject.ToLower()).FirstOrDefault();
                cek.Minimum = request.Minimum;
                cek.Medium = request.Medium;
                cek.Maximum = request.Maximum;
                cek.CreatedAt = DateTime.UtcNow;
                cek.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Setting>().UpdateAsync(cek);
                await _unitOfWork.Save(cancellationToken);
            }
            else
            {
                settings.MachineName = request.Name;
                settings.SubjectName = request.Subject;
                settings.Minimum = request.Minimum;
                settings.Medium = request.Medium;
                settings.Maximum = request.Maximum;
                settings.CreatedAt = DateTime.UtcNow;
                settings.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Setting>().AddAsync(settings);
                settings.AddDomainEvent(new SettingCreatedEvent(settings));
                await _unitOfWork.Save(cancellationToken);
            }

            var settingResponse = _mapper.Map<CreateSettingResponseDto>(settings);
            return await Result<CreateSettingResponseDto>.SuccessAsync(settingResponse, "Setting Created");
        }

    }
}

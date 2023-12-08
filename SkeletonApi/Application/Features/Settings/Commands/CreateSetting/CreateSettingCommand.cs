using AutoMapper;
using MediatR;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Settings.Commands.CreateSetting
{
    public record CreateSettingCommand : IRequest<Result<Setting>>, IMapFrom<Setting>
    {
        [JsonPropertyName("machine")]
        public string Name { get; set; }
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
        [JsonPropertyName("minimum")]
        public decimal? Minimum { get; set; }
        [JsonPropertyName("medium")]
        public decimal? Medium { get; set; }
        [JsonPropertyName("maximum")]
        public decimal? Maximum { get; set; }
    }
    internal class CreateSettingCommandHandler : IRequestHandler<CreateSettingCommand, Result<Setting>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSettingCommandHandler(IUnitOfWork unitOfWork,  IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Setting>> Handle(CreateSettingCommand request, CancellationToken cancellationToken)
        {
            var setting = new Setting()
            {

                MachineName = request.Name,
                SubjectName = request.Subject,
                Minimum = request.Minimum,
                Medium = request.Medium,
                Maximum = request.Maximum,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<Setting>().AddAsync(setting);
            setting.AddDomainEvent(new SettingCreatedEvent(setting));
            await _unitOfWork.Save(cancellationToken);
            return await Result<Setting>.SuccessAsync(setting, "Machines Created");
        }
    }
}

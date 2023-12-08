using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Common.Mappings;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.MaintenanceCorrective.Commands.Create
{
    public record CreateMaintCorrectiveCommand : IRequest<Result<CreateMaintCorrectiveDto>>, IMapFrom<MaintCorrective>
    {
        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        [JsonPropertyName("machine_id")]
        public Guid? MachineId { get; set; }

        [JsonPropertyName("actual")]
        public string Actual { get; set; }

        [JsonPropertyName("start_date")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateOnly EndtDate { get; set; }

    }

    internal class CreateMaintCorrectiveCommandHandler : IRequestHandler<CreateMaintCorrectiveCommand, Result<CreateMaintCorrectiveDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateMaintCorrectiveCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CreateMaintCorrectiveDto>> Handle(CreateMaintCorrectiveCommand request, CancellationToken cancellationToken)
        {

            // Periksa apakah Machine dengan ID yang diberikan ada
            var machine = await _unitOfWork.Repository<Machine>().Entities
                .Where(x => x.Id == request.MachineId)
                .FirstOrDefaultAsync(cancellationToken);

            if (machine != null)
            {
                var maintenance = new MaintCorrective()
                {
                    Id = Guid.NewGuid(),
                    Actual = request.Actual,
                    StartDate = request.StartDate,
                    EndDate = request.EndtDate,
                    MachineId = request.MachineId,
                    CreatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.Repository<MaintCorrective>().AddAsync(maintenance);
                maintenance.AddDomainEvent(new CreateMaintCorrectiveEvent(maintenance));

                await _unitOfWork.Save(cancellationToken);

                var data = new CreateMaintCorrectiveDto()
                {
                    Id = maintenance.Id,
                    MachineId = machine.Id,
                    Name = machine.Name,
                    StartDate = (DateOnly)maintenance.StartDate,
                    Actual = maintenance.Actual,
                    EndDate = maintenance.EndDate,
                };

                return Result<CreateMaintCorrectiveDto>.Success(data, "Maintenance Preventive Created");
            }
            else
            {
                return Result<CreateMaintCorrectiveDto>.Failure("Machine Not Found");
            }
        }

    }
}

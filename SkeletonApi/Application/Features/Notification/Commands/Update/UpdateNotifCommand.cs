using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Domain.Entities;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkeletonApi.Application.Features.Notification.Commands.Update
{
    public record class UpdateNotifCommand : IRequest<Result<UpdateNotifDto>>
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; }
    }

    internal class UpdateNotifCommandHandler : IRequestHandler<UpdateNotifCommand, Result<UpdateNotifDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateNotifCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UpdateNotifDto>> Handle(UpdateNotifCommand request, CancellationToken cancellationToken)
        {
            var notif = await _unitOfWork.Repository<Notifications>().GetByIdAsync(request.Id);
            if (notif != null)
            {
                notif.Status = request.Status;
                notif.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Notifications>().UpdateAsyncById(notif, request.Id);
                notif.AddDomainEvent(new UpdateNotifEvent(notif));
                await _unitOfWork.Save(cancellationToken);
                var Data = _mapper.Map<UpdateNotifDto>(notif);
                return await Result<UpdateNotifDto>.SuccessAsync(Data, "Notificatons Updated");
            }
            return await Result<UpdateNotifDto>.FailureAsync("Notificatons Not Found");
        }
    }
}

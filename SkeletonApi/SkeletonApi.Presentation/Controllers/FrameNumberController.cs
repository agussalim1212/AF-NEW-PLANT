using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.FrameNumb;
using SkeletonApi.Application.Features.FrameNumb.Commands.CreateFrameNumber;
using SkeletonApi.Shared;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/frame-number")]
    public class FrameNumberController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public FrameNumberController(IMediator mediator, ILogger<FrameNumberController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<CreateFrameNumberResponseDto>>> Create(CreateFrameNumberRequest command)
        {
            return await _mediator.Send(command);
        }

    }



}

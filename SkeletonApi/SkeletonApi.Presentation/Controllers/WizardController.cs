using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("api/wizard")]
    public class WizardController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private ILogger _logger;

        public WizardController(IMediator mediator, ILogger<WizardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("wizard")]
        public async Task<ActionResult> Wizard(string path)
        {
            Process.Start(path);
            return Ok(path);
        }
    }
}
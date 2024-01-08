using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkeletonApi.Application.Features.Machines.Commands.CreateMachines;
using SkeletonApi.Application.Features.Machines;
using SkeletonApi.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

           // StreamReader reader = new StreamReader("C:\\wizard\\Wizard_EID\\wpf_wiz_af.db");

            Process.Start(path, "C:\\wizard\\Wizard_EID\\wpf_wiz_af.db");
            return Ok(path);
        }
    }
}

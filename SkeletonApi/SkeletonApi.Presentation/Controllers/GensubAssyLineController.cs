using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonApi.Presentation.Controllers
{
    [Route("/api/gensub-assy-line")]
    public class GensubAssyLineController
    {
        private readonly IMediator _mediator;
        private ILogger _logger;
        public GensubAssyLineController(IMediator mediator, ILogger<MachineController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
    }
}

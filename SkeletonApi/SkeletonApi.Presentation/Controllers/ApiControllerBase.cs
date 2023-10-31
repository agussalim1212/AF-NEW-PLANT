using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SkeletonApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("text/json")]
    public abstract class ApiControllerBase : ControllerBase
    {

    }
}
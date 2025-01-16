using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
    }
}

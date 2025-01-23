using Application.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EnumController(IEnumService enumService) : BaseController
    {
        [HttpGet("priority")]
        public IActionResult GetPriorityList()
        {
            return Ok(enumService.GetPriorities());
        }

        [HttpGet("status")]
        public IActionResult GetStatusList()
        {
            return Ok(enumService.GetStatus());
        }
    }
}

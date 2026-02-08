using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Success";
        }
    }
}

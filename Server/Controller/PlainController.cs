using Microsoft.AspNetCore.Mvc;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class PlainController : ControllerBase
    {
        [HttpGet]
        public IActionResult PlainText()
        {
            var helloWorld = "Hello, World!";

            Response.ContentType = "text/plain";
            Response.ContentLength = helloWorld.Length;

            return Content(helloWorld);
        }
    }
}

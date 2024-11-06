using Microsoft.AspNetCore.Mvc;

namespace Server.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class PlainController : ControllerBase
    {
        private static readonly string helloWorld = "Hello, World!";

        [HttpGet]
        public IActionResult PlainText()
        {
            Response.ContentType = "text/plain";
            Response.ContentLength = helloWorld.Length;

            return Content(helloWorld);
        }
    }
}

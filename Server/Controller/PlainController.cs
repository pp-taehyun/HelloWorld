using Microsoft.AspNetCore.Mvc;
using Server;

namespace JsonSerialization.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class PlainController : ControllerBase
    {
        [HttpGet]
        public ActionResult PlainText()
        {
            var helloWorld = "Hello, World!";

            Response.ContentType = "text/plain";
            Response.ContentLength = helloWorld.Length;

            return Content(helloWorld);
        }
    }
}

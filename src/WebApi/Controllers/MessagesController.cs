using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    [HttpGet]
    public ActionResult Hello(
        [FromQuery] string name)
    {
        return Ok($"Hello, {name}");
    }
} 
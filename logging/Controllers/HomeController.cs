namespace logging.Controllers;

using logging.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class HomeController : ControllerBase
{
    public HomeController() { }

    [HttpGet("/ok")]
    public IActionResult Index()
    {
        // return Ok("OK");
        var a = 100;
        var b = 0;

        try
        {
            var c = a / b;
            return Ok(c);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.ToString());
        }
    }

    [HttpGet("/no-content")]
    public IActionResult Get()
    {
        return StatusCode(204);
    }

    [HttpPost]
    [Route("/login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId);
        return Ok(request);
    }

    [HttpPost]
    [Route("/exception-with-code")]
    public IActionResult ExceptionWithCode()
    {
        return StatusCode(500, new { Code = "NP0001", Message = "Internal server error!" });
    }
    
    
    [HttpPost]
    [Route("/exception-without-code")]
    public IActionResult ExceptionWithOutCode()
    {
        return StatusCode(500, new { Message = "Internal server error!" });
    }
}
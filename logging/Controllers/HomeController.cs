namespace logging.Controllers;

using logging.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class HomeController: ControllerBase
{
    public HomeController()
    {
        
    }
    
    [HttpGet("/ok")]
    public IActionResult Index()
    {
        return Ok("OK");
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
        return Ok(correlationId);
    }
}
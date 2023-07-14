namespace timed_hosted_service.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public sealed class MessageController : ControllerBase
{
    private readonly ILogger<MessageController> _logger;

    public MessageController(ILogger<MessageController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("Get called");
        return Ok("Hello World");
    }
}
using Microsoft.AspNetCore.Mvc;
using ShotTron.Server.Models;

namespace ShotTron.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;

    public SessionController(ILogger<SessionController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "CreateSession")]
    public CreateSessionResultDto CreateSession(CreateSessionDto createDto)
    {
        return new CreateSessionResultDto("1234");
    }
}
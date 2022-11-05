using Microsoft.AspNetCore.Mvc;

namespace ShotTron.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ShotTronController : ControllerBase
{
    private readonly ILogger<ShotTronController> _logger;

    public ShotTronController(ILogger<ShotTronController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<ShotTronController> Get()
    {
        return null;
    }
}
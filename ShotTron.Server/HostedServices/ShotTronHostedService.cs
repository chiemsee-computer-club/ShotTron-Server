namespace ShotTron.Server.HostedServices;

public class ShotTronHostedService : IntervalHostedService
{
    private readonly ILogger<ShotTronHostedService> _logger;

    public ShotTronHostedService(
        ILogger<ShotTronHostedService> logger
        ) : base(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1))
    {
        _logger = logger;
    }
    
    public override Task<bool> DoWorkAsync()
    {
        throw new NotImplementedException();
    }
}
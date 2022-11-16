using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using ShotTron.Server.Hubs;
using ShotTron.Server.Services;

namespace ShotTron.Server.HostedServices;

public class EventTriggerHostedService : IntervalHostedService
{
    private readonly ILogger<EventTriggerHostedService> _logger;
    private readonly ICacheRepository _cacheRepository;
    private readonly IHubContext<SessionHub> _hubContext;

    public EventTriggerHostedService(
        ILogger<EventTriggerHostedService> logger,
        ICacheRepository cacheRepository,
        IHubContext<SessionHub> hubContext) : base(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))
    {
        _logger = logger;
        _cacheRepository = cacheRepository;
        _hubContext = hubContext;
    }
    
    public override async Task<bool> DoWorkAsync()
    {
        var sessions = _cacheRepository.GetAllSessions();
        var now = DateTimeOffset.UtcNow;
        var sessionsToSendEvent = sessions
            .Where(x => x?.NextEvent != null && x.NextEvent.TimeStamp >= now);

        foreach (var item in sessionsToSendEvent)
        {
            await _hubContext.Clients.Group(item.Id)
                .SendAsync(nameof(ISessionClient.NewEvent), item.NextEvent);

            _cacheRepository.NextEventSent(item.Id);
        }
        
        return true;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShotTron.Server.Extensions;
using ShotTron.Server.Services;

namespace ShotTron.Server.Hubs;

[Authorize]
public class SessionHub : Hub<ISessionClient>
{
    private readonly ICacheRepository _cacheRepository;
    public const string Endpoint = "/events";

    public SessionHub(ICacheRepository cacheRepository)
    {
        _cacheRepository = cacheRepository;
    }
    
    public async Task SubscribeEvents()
    {
        var nickname = Context.User.GetNickname();
        var sessionId = Context.User.GetSessionId();

        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        _cacheRepository.SetSignalRConnectionId(sessionId, nickname, Context.ConnectionId);
    }
}
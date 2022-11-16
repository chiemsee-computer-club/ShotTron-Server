using Microsoft.Extensions.Caching.Memory;
using ShotTron.Server.Models;

namespace ShotTron.Server.Services;

public class CacheRepository : ICacheRepository
{
    private const string SessionsKey = "Sessions";
    private const string SessionPrefixKey = "Session_";
    private const int SessionKeyLength = 5;
        
    
    private readonly IMemoryCache _memoryCache;

    public CacheRepository(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Session[] GetAllSessions()
    {
        var ret = GetSessionIds()
            .Select(GetSession);

        return ret.ToArray();
    }
    
    public string CreateSession(TimeSpan eventIntervalMin, TimeSpan eventIntervalMax)
    {
        var sessions = _memoryCache.GetOrCreate<List<string>>(SessionsKey, entry => new List<string>());

        var session = new Session(eventIntervalMin, eventIntervalMax);
        sessions!.Add(session.Id);

        SaveSession(session);
        _memoryCache.Set(SessionsKey, sessions);

        return session.Id;
    }

    public void CreatePlayer(string sessionId, string nickname)
    {
        var session = GetSession(sessionId);

        if (session == null || session.Players.Any(x => x.Nickname == nickname))
        {
            throw new Exception();
        }

        var player = new Player(nickname);
        
        session.JoinSession(player);

        SaveSession(session);
    }
    
    public void SetSignalRConnectionId(string sessionId, string nickname, string connectionId)
    {
        var session = GetSession(sessionId);

        var player = session.Players.Single(x => x.Nickname == nickname);
        player.SignalRConnectionId = connectionId;
        
        SaveSession(session);
    }

    public void NextEventSent(string sessionId)
    {
        var session = GetSession(sessionId);
        session.SessionEvents.Add(session.NextEvent);

        var players = session.Players.Where(x => x.SignalRConnectionId != null);
        foreach (var item in players)
        {
            item.ReceivedEvents.Add(session.NextEvent.Id);
        }
        
        session.CalculateNextEvent();
    }
    
    public void ReactionToEvent(string sessionId, string nickname, Guid eventId)
    {
        var session = GetSession(sessionId);

        var player = session.Players.Single(x => x.Nickname == nickname);
        if (player.ReceivedEvents.Contains(eventId))
        {
            var eventReaction = new EventReaction
            {

                EventId = eventId,
                Timestamp = DateTimeOffset.Now
            };
            
            player.EventReactions.Add(eventReaction);
        }
    }

    private List<string> GetSessionIds() =>
        _memoryCache!.Get<List<string>>(SessionsKey)!;
    
    private Session GetSession(string sessionId) =>
        _memoryCache!.Get<Session>(SessionPrefixKey + sessionId)!;

    private void SaveSession(Session session)
    {
        var sessionId = session.Id;
        _memoryCache.Set(SessionPrefixKey + sessionId, session);
    }
}
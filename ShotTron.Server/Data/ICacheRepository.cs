using ShotTron.Server.Models;

namespace ShotTron.Server.Services;

public interface ICacheRepository
{
    public Session[] GetAllSessions();
    public string CreateSession(TimeSpan eventIntervalMin, TimeSpan eventIntervalMax);
    public void CreatePlayer(string sessionId, string nickname);
    public void SetSignalRConnectionId(string sessionId, string nickname, string connectionId);
    public void NextEventSent(string sessionId);
    public void ReactionToEvent(string sessionId, string nickname, Guid eventId);
}
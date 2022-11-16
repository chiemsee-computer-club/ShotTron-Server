namespace ShotTron.Server.Models;

public record Session
{
    private static Random _random = new Random();
    private const int SessionKeyLength = 5;
    
    private TimeSpan _eventIntervalMin;

    private TimeSpan _eventIntervalMax;

    public Session(TimeSpan eventIntervalMin, TimeSpan eventIntervalMax) 
    {
        Id = GenerateSessionId();
        _eventIntervalMin = eventIntervalMin;
        _eventIntervalMax = eventIntervalMax;
    }
    
    public string Id { get; set; }

    public SessionEvent NextEvent { get; private set; }
    
    public List<SessionEvent> SessionEvents { get; } = new List<SessionEvent>();

    public List<Player> Players { get; } = new List<Player>();

    public void CalculateNextEvent()
    {
        var nextEventDelaySec = _random.Next((int)_eventIntervalMin.TotalSeconds, (int)_eventIntervalMax.TotalSeconds);
        var now = DateTimeOffset.UtcNow;

        var nextTimeStamp = now + TimeSpan.FromSeconds(nextEventDelaySec);

        NextEvent = new SessionEvent
        {
            Id = Guid.NewGuid(),
            TimeStamp = nextTimeStamp,
            Type = GameEventType.Silent
        };
    }

    public void JoinSession(Player player)
    {
        Players.Add(player);
    }
    
    private string GenerateSessionId()
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(validChars, SessionKeyLength)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
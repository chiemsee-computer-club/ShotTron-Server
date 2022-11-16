namespace ShotTron.Server.Models;

public class SessionEvent
{
    public Guid Id { get; set; }
    public GameEventType Type { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
}
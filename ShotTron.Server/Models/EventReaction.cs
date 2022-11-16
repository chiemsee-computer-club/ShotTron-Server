namespace ShotTron.Server.Models;

public class EventReaction
{
    public Guid EventId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
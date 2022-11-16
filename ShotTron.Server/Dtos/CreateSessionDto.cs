namespace ShotTron.Server.Models;

public class CreateSessionDto
{
    public TimeSpan? EventIntervalMin  { get; set; }
    public TimeSpan? EventIntervalMax  { get; set; }

    public string Nickname { get; set; }
}
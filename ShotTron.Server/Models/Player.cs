namespace ShotTron.Server.Models;

public record Player
{
    public Player(string nickname)
    {
        if (nickname == null || nickname.Length > 20)
        {
            throw new Exception();
        }
        
        Nickname = nickname;
    }
    
    public string Nickname { get; }

    public DateTimeOffset Joined { get; set; }

    public string? SignalRConnectionId { get; set; } = null;
    
    public List<Guid> ReceivedEvents { get; } = new();
    public List<EventReaction> EventReactions { get; set; }
}
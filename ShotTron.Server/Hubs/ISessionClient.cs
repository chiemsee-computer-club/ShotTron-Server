using ShotTron.Server.Models;

namespace ShotTron.Server.Hubs;

public interface ISessionClient
{
    Task NewEvent(NewEventDto eventDto);
}
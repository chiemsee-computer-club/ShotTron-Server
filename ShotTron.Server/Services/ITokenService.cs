namespace ShotTron.Server.Services;

public interface ITokenService
{
    public string GenerateToken(string sessionId, string nickname, string role);
}
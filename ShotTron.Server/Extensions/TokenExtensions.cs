using System.Security.Claims;
using ShotTron.Server.Models;

namespace ShotTron.Server.Extensions;

public static class TokenExtensions
{
    public const string SessionIdClaim = "SessionId";
    public const string NicknameClaim = "Nickname";
    public const string RoleClaim = "Role";
    
    public static string GetSessionId(this ClaimsPrincipal claimsPrincipal)
    {
        var nicknameClaim = claimsPrincipal.Claims
            .Single(x => x.Type == SessionIdClaim);

        return nicknameClaim.Value;
    }
    
    public static string GetNickname(this ClaimsPrincipal claimsPrincipal)
    {
        var nicknameClaim = claimsPrincipal.Claims
            .Single(x => x.Type == NicknameClaim);

        return nicknameClaim.Value;
    }
    
    public static PlayerRole GetRole(this ClaimsPrincipal claimsPrincipal)
    {
        var nicknameClaim = claimsPrincipal.Claims
            .Single(x => x.Type == RoleClaim);

        return Enum.Parse<PlayerRole>(nicknameClaim.Value);
    }
}
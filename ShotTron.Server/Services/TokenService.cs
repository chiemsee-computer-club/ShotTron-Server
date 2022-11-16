using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShotTron.Server.Extensions;
using ShotTron.Server.Models;

namespace ShotTron.Server.Services;

public class TokenService : ITokenService
{
    private readonly AppConfig _appConfig;

    public TokenService(AppConfig appConfig)
    {
        _appConfig = appConfig;
    }
    
    public string GenerateToken(string sessionId, string nickname, string role)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.JwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(TokenExtensions.SessionIdClaim, sessionId),
            new Claim(TokenExtensions.NicknameClaim,nickname),
            new Claim(TokenExtensions.RoleClaim,role)
        };
        
        var token = new JwtSecurityToken(_appConfig.JwtIssuer,
            _appConfig.JwtAudience,
            claims,
            expires: DateTime.Now.AddYears(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
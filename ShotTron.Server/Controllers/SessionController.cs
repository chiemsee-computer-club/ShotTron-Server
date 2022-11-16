using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShotTron.Server.Extensions;
using ShotTron.Server.Models;
using ShotTron.Server.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ShotTron.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{
    private readonly ILogger<SessionController> _logger;
    private readonly ICacheRepository _cacheRepository;
    private readonly AppConfig _appConfig;
    private readonly ITokenService _tokenService;

    public SessionController(
        ILogger<SessionController> logger,
        ICacheRepository cacheRepository,
        AppConfig appConfig,
        ITokenService tokenService
        )
    {
        _logger = logger;
        _cacheRepository = cacheRepository;
        _appConfig = appConfig;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost(Name = "CreateSession")]
    [SwaggerOperation(OperationId = "createSession")]
    public JoinSessionResultDto CreateSession(CreateSessionDto createDto)
    {
        var sessionId = _cacheRepository.CreateSession(createDto.EventIntervalMin ?? TimeSpan.FromSeconds(30), createDto.EventIntervalMax ?? TimeSpan.FromSeconds(60));
        
        _cacheRepository.CreatePlayer(sessionId, createDto.Nickname);

        var token = _tokenService.GenerateToken(sessionId, createDto.Nickname, "Owner");

        return new JoinSessionResultDto(sessionId, token);
    }
    
    [AllowAnonymous]
    [HttpPost("Join", Name = "JoinSession")]
    [SwaggerOperation(OperationId = "joinSession")]
    public JoinSessionResultDto JoinSession(JoinSessionDto joinDto)
    {
        var sessionId = joinDto.SessionId;
        var nickname = joinDto.Nickname;
        
        _cacheRepository.CreatePlayer(sessionId, nickname);

        var token = _tokenService.GenerateToken(sessionId, nickname, "Player");
        return new JoinSessionResultDto(sessionId, token);
    }

    [HttpPost("EventReaction", Name = "EventReaction")]
    [SwaggerOperation(OperationId = "postSessionEventReaction")]
    public EventReactionResultDto PostEventReaction(EventReactionDto reactionDto)
    {
        var nickname = User.GetNickname();
        var sessionId = User.GetSessionId();
        
        _cacheRepository.ReactionToEvent(sessionId, nickname, reactionDto.EventId);

        return new EventReactionResultDto(1);
    }
}
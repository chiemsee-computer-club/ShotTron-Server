using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShotTron.Server.Hubs;
using ShotTron.Server.Models;
using ShotTron.Server.Services;

namespace ShotTron.Server;

public class Startup
{
    private readonly AppConfig _appConfig;
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
        _appConfig = configuration
            .GetSection(nameof(AppConfig))
            .Get<AppConfig>() ?? new AppConfig();
        
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_appConfig);
        
        // services.AddDbContext<ShotTronDbContext>(
        //     options => options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

        services.AddMemoryCache();
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _appConfig.JwtIssuer,
                ValidAudience = _appConfig.JwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(_appConfig.JwtSecret)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
            
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments(SessionHub.Endpoint))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });
        
        services.AddAuthorization();

        services.AddSingleton<ICacheRepository, CacheRepository>();
        services.AddSingleton<ITokenService, TokenService>();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "ShotTron3000 API",
                    Description = "ShotTron3000 API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Jakob Bauer",
                        Url = new Uri("https://www.bauer-jakob.de"),
                        Email = "info@bauer-jakob.de"
                    }
                });
        });

        services.AddSignalR();
    }
    
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ShotTron API");
        });

        app.UseEndpoints(e =>
            {
                e.MapHub<SessionHub>(SessionHub.Endpoint);
            }
        );
    }
}
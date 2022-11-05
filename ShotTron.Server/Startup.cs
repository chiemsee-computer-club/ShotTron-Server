using Microsoft.OpenApi.Models;
using ShotTron.Server.Models;

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

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        
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
    }
}
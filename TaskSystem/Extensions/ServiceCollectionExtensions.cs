using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using TaskSystem.DataAccessLayer;
using TaskSystem.Domain.Interfaces;
using TaskSystem.Domain.Interfaces.Providers;
using TaskSystem.Domain.Models;
using TaskSystem.Domain.Providers;
using TaskSystem.Domain.Validation;
using TaskSystem.Models.Auth;

namespace TaskSystem.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection
        services, IConfiguration configuration, string name)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: name,
                b =>
                {
                    b.WithOrigins(configuration.GetSection("CORS:Origins").Get<string[]>() ?? throw new InvalidOperationException())
                        .WithHeaders(configuration.GetSection("CORS:Headers").Get<string[]>() ?? throw new InvalidOperationException())
                        .WithMethods(configuration.GetSection("CORS:Methods").Get<string[]>() ?? throw new InvalidOperationException())
                        .AllowCredentials();
                });
        });

        return services;
    }
    
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection
        services, IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("EntityContext");

        services.AddDbContext<ServiceDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IServiceDbContext>(provider =>
            provider.GetService<ServiceDbContext>() ?? throw new InvalidOperationException());

        services.AddScoped<ITaskTicketCrudProvider, TaskTicketCrudProvider>();
        services.AddScoped<ITaskHistoryCrudProvider, TaskHistoryCrudProvider>();


        return services;
    }
    
    public static IServiceCollection AddSerilogLogging(this IServiceCollection
        services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Logger(lc => lc
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
            )
            .WriteTo.Logger(lc => lc
                .WriteTo.File($"Logs{Path.DirectorySeparatorChar}OpcUaClientAppLog-.txt", rollingInterval: RollingInterval.Day)
            )
            .CreateLogger();
        
        services.AddSingleton(Log.Logger);
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger);
        });
        
        return services;
    }
    
    public static IServiceCollection AddFluentValidation(this IServiceCollection
        services)
    {
        services.AddValidatorsFromAssemblyContaining<TaskTicketValidator>();
        services.AddValidatorsFromAssemblyContaining<TaskHistoryValidator>();

        
        return services;
    }
    
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<JwtSettings>(cfg.GetSection("Jwt"));
        var jwt = cfg.GetRequiredSection("Jwt").Get<JwtSettings>()!;

        services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                    ClockSkew = TimeSpan.Zero
                    
                };
                
                opts.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        var logger = ctx.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JWT");
                        logger.LogWarning(ctx.Exception, "JWT auth failed");
                        return Task.CompletedTask;
                    }
                };
                opts.TokenValidationParameters.RoleClaimType = "Role";
                opts.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Sub;
            });

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddHttpContextAccessor();
        return services;
    }
    
    public static IServiceCollection AddJwtSwagger(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name   = "Authorization",
                In     = ParameterLocation.Header,
                Type   = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        return services;
    }
}
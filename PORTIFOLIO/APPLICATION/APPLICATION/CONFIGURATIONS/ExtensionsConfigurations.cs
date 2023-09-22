using APPLICATION.APPLICATION.CONFIGURATIONS.APPLICATIONINSIGHTS;
using APPLICATION.APPLICATION.CONFIGURATIONS.SWAGGER;
using APPLICATION.APPLICATION.SERVICES.CHAT;
using APPLICATION.APPLICATION.SERVICES.FILE;
using APPLICATION.APPLICATION.SERVICES.JOBS;
using APPLICATION.APPLICATION.SERVICES.TOKEN;
using APPLICATION.APPLICATION.SERVICES.USER;
using APPLICATION.DOMAIN.CONTRACTS.API;
using APPLICATION.DOMAIN.CONTRACTS.CONFIGURATIONS;
using APPLICATION.DOMAIN.CONTRACTS.CONFIGURATIONS.APPLICATIONINSIGHTS;
using APPLICATION.DOMAIN.CONTRACTS.FACADE;
using APPLICATION.DOMAIN.CONTRACTS.FEATUREFLAGS;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.CHAT;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.EVENTS;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.USER;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.CHAT;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.FILE;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.JOBS;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.TOKEN;
using APPLICATION.DOMAIN.CONTRACTS.SERVICES.USER;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.UTILS.GLOBAL;
using APPLICATION.DOMAIN.UTILS.JOBMETHODS;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using APPLICATION.INFRAESTRUTURE.FACADES;
using APPLICATION.INFRAESTRUTURE.FEATUREFLAGS;
using APPLICATION.INFRAESTRUTURE.JOBS.FACTORY.FLUENTSCHEDULER;
using APPLICATION.INFRAESTRUTURE.JOBS.INTERFACES.BASE;
using APPLICATION.INFRAESTRUTURE.REPOSITORY;
using APPLICATION.INFRAESTRUTURE.REPOSITORY.BASE;
using APPLICATION.INFRAESTRUTURE.SERVICEBUS.PROVIDER.USER;
using APPLICATION.INFRAESTRUTURE.SERVICEBUS.SUBSCRIBER.USER;
using Hangfire;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Refit;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Globalization;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using static APPLICATION.DOMAIN.EXCEPTIONS.USER.CustomUserException;

namespace APPLICATION.APPLICATION.CONFIGURATIONS;

/// <summary>
/// Extensions
/// </summary>
public static class ExtensionsConfigurations
{
    public static readonly string HealthCheckEndpoint = "/application/healthcheck";

    private static string _applicationInsightsKey;

    private static TelemetryConfiguration _telemetryConfig;

    private static TelemetryClient _telemetryClient;

    /// <summary>
    /// Configuração de Logs do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureSerilog(this IServiceCollection services, IConfiguration configurations)
    {
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .WriteTo.Console()
                .WriteTo.ApplicationInsights(_telemetryConfig, TelemetryConverter.Traces, LogEventLevel.Information)
                .WriteTo.MSSqlServer(configurations.GetValue<string>("ConnectionStrings:BaseDados"), new MSSqlServerSinkOptions
                {
                    AutoCreateSqlDatabase = true,
                    TableName = "Logs"
                })
                .CreateLogger();

        var applicationInsightsServiceOptions = new ApplicationInsightsServiceOptions
        {
            EnableAdaptiveSampling = false,
            EnableDependencyTrackingTelemetryModule = false,
            EnableRequestTrackingTelemetryModule = false
        };

        services
            .AddTransient<ILogWithMetric, LogWithMetric>()
            .AddApplicationInsightsTelemetry(applicationInsightsServiceOptions);

        return services;
    }

    /// <summary>
    /// Configuração de linguagem principal do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureLanguage(this IServiceCollection services)
    {
        var cultureInfo = new CultureInfo("pt-BR");

        CultureInfo
            .DefaultThreadCurrentCulture = cultureInfo;

        CultureInfo
            .DefaultThreadCurrentUICulture = cultureInfo;

        return services;
    }

    /// <summary>
    /// Configuração do banco de dados do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureContexto(this IServiceCollection services, IConfiguration configurations)
    {
        services
            .AddDbContext<Context>(options =>
            {
                options.UseSqlServer(configurations.GetValue<string>("ConnectionStrings:BaseDados")).LogTo(Console.WriteLine, LogLevel.None);

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            }, ServiceLifetime.Scoped);

        services
           .AddDbContext<LazyLoadingContext>(options =>
           {
               options.UseLazyLoadingProxies().UseSqlServer(configurations.GetValue<string>("ConnectionStrings:BaseDados")).LogTo(Console.WriteLine, LogLevel.None);

           }, ServiceLifetime.Scoped);

        return services;
    }

    /// <summary>
    /// Configuração do identity server do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;

                options.SignIn.RequireConfirmedAccount = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                options.User.RequireUniqueEmail = true;

                options.Stores.MaxLengthForKeys = 20;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                options.Lockout.MaxFailedAccessAttempts = 3;

                options.Lockout.AllowedForNewUsers = true;

                options.Password.RequireDigit = true;

                options.Password.RequireLowercase = true;

                options.Password.RequireUppercase = true;

                options.Password.RequiredLength = configuration.GetValue<int>("Auth:Password:RequiredLength");

                options.Password.RequireNonAlphanumeric = true;

                options.Password.RequiredUniqueChars = 1;

            }).AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Configuração da autenticação do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurations"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultScheme = IdentityConstants.ApplicationScheme;

        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = configurations.GetValue<string>("Auth:ValidIssuer"),
                ValidAudience = configurations.GetValue<string>("Auth:ValidAudience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configurations.GetValue<string>("Auth:SecurityKey")))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Log.Error($"[LOG ERROR] {nameof(JwtBearerEvents)} - METHOD OnAuthenticationFailed - {context.Exception.Message}\n");

                    throw new UnauthorizedUserException(null);
                },

                OnTokenValidated = context =>
                {
                    Log.Information($"[LOG INFORMATION] {nameof(JwtBearerEvents)} - OnTokenValidated - {context.SecurityToken}\n");

                    if (!context.Principal.Claims.Any(claim => claim.Type.Equals("id")))
                    {
                        GlobalData.GlobalUser = new DOMAIN.DTOS.USER.UserData
                        {
                            Id = Guid.Parse(context.Principal.Claims?.FirstOrDefault().Value)
                        };
                    }

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Configuração da authorização do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurations"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Configura os cookies da applicação.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureApllicationCookie(this IServiceCollection services)
    {
        return services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;

            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            options.SlidingExpiration = true;
        });
    }

    /// <summary>
    /// Configuração de métricas
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var httpContextAccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();

        _telemetryConfig = TelemetryConfiguration.CreateDefault();

        _telemetryConfig.ConnectionString = configuration.GetSection("ApplicationInsights:ConnectionStringApplicationInsightsKey").Value;

        _telemetryConfig.TelemetryInitializers.Add(new ApplicationInsightsInitializer(configuration, httpContextAccessor));

        _telemetryClient = new TelemetryClient(_telemetryConfig);

        services
            .AddSingleton<ITelemetryInitializer>(x => new ApplicationInsightsInitializer(configuration, httpContextAccessor))
            .AddSingleton<ITelemetryProxy>(x => new TelemetryProxy(_telemetryClient));

        return services;
    }

    /// <summary>
    /// Configuração de App Insights
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureApplicationInsights(this IServiceCollection services, IConfiguration configuration)
    {
        var metrics = new ApplicationInsightsMetrics(_telemetryClient, _applicationInsightsKey);

        var applicationInsightsServiceOptions = new ApplicationInsightsServiceOptions
        {
            ConnectionString = configuration.GetSection("ApplicationInsights:ConnectionStringApplicationInsightsKey").Value
        };

        services
            .AddApplicationInsightsTelemetry(applicationInsightsServiceOptions)
            .AddTransient(x => metrics)
            .AddTransient<IApplicationInsightsMetrics>(x => metrics);

        return services;
    }

    /// <summary>
    /// Configuração do swagger do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurations"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configurations)
    {
        var apiVersion = configurations.GetValue<string>("SwaggerInfo:ApiVersion"); 
        var apiDescription = configurations.GetValue<string>("SwaggerInfo:ApiDescription"); 
        var description = configurations.GetValue<string>("SwaggerInfo:Description"); 
        var uriMyGit = configurations.GetValue<string>("SwaggerInfo:UriMyGit");

        services.AddSwaggerGen(swagger =>
        {
            swagger.EnableAnnotations();

            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            swagger.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Version = apiVersion,
                Title = $"{apiDescription} - {apiVersion}",
                Description = description,

                Contact = new OpenApiContact
                {
                    Name = "HYPER.IO DESENVOLVIMENTOS LTDA",
                    Email = "HYPER.IO@OUTLOOK.COM",
                },
                License = new OpenApiLicense
                {
                    Name = "HYPER.IO LICENSE",

                },
                TermsOfService = new Uri(uriMyGit)
            });

            swagger.DocumentFilter<HealthCheckSwagger>();
        });

        return services;
    }

    /// <summary>
    /// Configuração das dependencias (Serrvices, Repository, Facades, etc...).
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfiguration configurations, IWebHostEnvironment webHostEnvironment)
    {
        if (webHostEnvironment.IsProduction())
        {
            if (string.IsNullOrEmpty(configurations.GetValue<string>("ApplicationInsights:InstrumentationKey")))
            {
                var argNullEx = new ArgumentNullException("AppInsightsKey não pode ser nulo.", new Exception("Parametro inexistente.")); throw argNullEx;
            }
            else
            {
                _applicationInsightsKey = configurations.GetValue<string>("ApplicationInsights:InstrumentationKey");
            }
        }

        services
            .AddSingleton(serviceProvider => configurations)
            // Services
            .AddTransient<IUserService, UserService>()
            .AddTransient<ITokenService, TokenService>()
            .AddTransient<IFileService, FileService>()
            .AddTransient<IChatService, ChatSertvice>()
            // Facades
            .AddSingleton<IUtilFacade, UtilFacade>()
            // Repository
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped(typeof(IGenerictEntityCoreRepository<>), typeof(GenericEntityCoreRepository<>))
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IChatRepository, ChatRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            // Infra
            .AddSingleton<IUserEmailServiceBusSenderProvider, UserEmailServiceBusSenderProvider>()
            .AddSingleton<IUserEmailServiceBusReceiverProvider, UserEmailServiceBusReceiverProvider>()
            .AddScoped<IFeatureFlags, FeatureFlagsProvider>()
            .AddScoped<LazyLoadingContext>();
            

        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }

    /// <summary>
    /// Configura chamadas a APIS externas através do Refit.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureRefit(this IServiceCollection services, IConfiguration configurations)
    {
        services
            .AddRefitClient<IExternalUtil>().ConfigureHttpClient(c => c.BaseAddress = configurations.GetValue<Uri>("UrlBase:TOOLS_UTIL_API"));

        return services;
    }

    /// <summary>
    /// Configuração do HealthChecks do sistema.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurations"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configurations)
    {
        services
            .AddHealthChecks().AddSqlServer(configurations.GetConnectionString("BaseDados"), name: "Base de dados padrão.", tags: new string[] { "Core", "SQL Server" });

        return services;
    }

    /// <summary>
    /// Configuração dos cors aceitos.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true).AllowCredentials();
            });
        });
    }

    /// <summary>
    /// Registro de Jobs.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureFluentSchedulerJobs(this IServiceCollection services)
    {
        services.AddTransient<IFluentSchedulerJobs, FluentSchedulerJobs>();

        //services.AddTransient<IResendFailedMailsJob, ProcessResendFailedMailsJob>();

        services.ConfigureStartJobs();

        return services;
    }

    /// <summary>
    /// Configure Hangfire
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureHangFire(this IServiceCollection services, IConfiguration configurations)
    {
        services.AddHangfire(configuration
            => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configurations.GetConnectionString("BaseDados")));

        services.AddTransient<IJobsService, HangFireJobService>();

        services.AddHangfireServer();

        return services;
    }

    /// <summary>
    /// Iniciar Jobs.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureStartJobs(this IServiceCollection services)
    {
        // Iniciar os Jobs.
        new ScheduledTasksManager(services.GetProvider()).StartJobs();

        return services;
    }

    // Configura os subscribers.
    public static IServiceCollection ConfigureSubscribers(this IServiceCollection services)
    {
        services
            //Subscribers
            .AddTransient<UserSubscriber>();

        return services;
    }

    /// <summary>
    /// Configuração do HealthChecks do sistema.
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder application)
    {
        application.UseHealthChecks(HealthCheckEndpoint, new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                var result = JsonConvert.SerializeObject(new
                {
                    statusApplication = report.Status.ToString(),

                    healthChecks = report.Entries.Select(e => new
                    {
                        check = e.Key,
                        ErrorMessage = e.Value.Exception?.Message,
                        status = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                    })
                });

                context.Response.ContentType = MediaTypeNames.Application.Json;

                await context.Response.WriteAsync(result);
            }
        });

        return application;
    }

    /// <summary>
    /// Coniguras os endpoints & Hubs
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    //public static IApplicationBuilder UseEndpoints(this IApplicationBuilder application)
    //{
    //    application.UseEndpoints(endpoints =>
    //    {
    //        endpoints.MapHub<HubPerson>("/person");
    //    });

    //    return application;
    //}

    /// <summary>
    /// Configuração de uso do swagger do sistema.
    /// </summary>
    /// <param name="application"></param>
    /// <param name="configurations"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerConfigurations(this IApplicationBuilder application, IConfiguration configurations)
    {
        var apiVersion = configurations.GetValue<string>("SwaggerInfo:ApiVersion");

        application.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        application
            .UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"{apiVersion}");
            });

        application
            .UseMvcWithDefaultRoute();

        return application;
    }

    /// <summary>
    /// Execute Seeds in database
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static async Task<IApplicationBuilder> Seeds(this IApplicationBuilder application, WebApplication webApplication)
    {
        using (var scope = webApplication.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            var context = scope.ServiceProvider.GetRequiredService<Context>();

            if (await userManager.Users.AnyAsync() is false)
            {
                Log.Debug($"[LOG DEBUG] - Iniciando seeds da aplicação.\n");

                // Set data in user.
                var user = new User
                {
                    FirstName = "Hyper",
                    LastName = "Teste",
                    Email = "Hyper.ip@outlook.com",
                    EmailConfirmed = true,
                    UserName = "User.Teste",
                    Created = DateTime.Now,
                    Status = Status.Active,
                };

                // Generate a password hash.
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Teste@123456");

                // Create user.
                await userManager.CreateAsync(user);

                // Update user.
                await userManager.UpdateAsync(user);

                // Add Login in user.
                await userManager.AddLoginAsync(user, new UserLoginInfo("TOOLS.USER.API", "TOOLS.USER", "TOOLS.USER.PROVIDER.KEY"));

                // Set data in role.
                var role = new Role
                {
                    Name = "administrator",
                    Status = Status.Active,
                    Created = DateTime.Now
                };

                // Create role.
                await roleManager.CreateAsync(role);

                // Add claim in role.
                await roleManager.AddClaimAsync(role, new Claim("User", "Get"));
                await roleManager.AddClaimAsync(role, new Claim("User", "Post"));
                await roleManager.AddClaimAsync(role, new Claim("User", "Put"));
                await roleManager.AddClaimAsync(role, new Claim("User", "Patch"));
                await roleManager.AddClaimAsync(role, new Claim("User", "Delete"));

                await roleManager.AddClaimAsync(role, new Claim("Claim", "Get"));
                await roleManager.AddClaimAsync(role, new Claim("Claim", "Post"));
                await roleManager.AddClaimAsync(role, new Claim("Claim", "Put"));
                await roleManager.AddClaimAsync(role, new Claim("Claim", "Patch"));
                await roleManager.AddClaimAsync(role, new Claim("Claim", "Delete"));

                await roleManager.AddClaimAsync(role, new Claim("Role", "Get"));
                await roleManager.AddClaimAsync(role, new Claim("Role", "Post"));
                await roleManager.AddClaimAsync(role, new Claim("Role", "Put"));
                await roleManager.AddClaimAsync(role, new Claim("Role", "Patch"));
                await roleManager.AddClaimAsync(role, new Claim("Role", "Delete"));

                // Add role to user.
                await userManager.AddToRoleAsync(user, role.Name);

                // Update user.
                await userManager.UpdateAsync(user);

                // Commit de transaction.
                await context.SaveChangesAsync();
            }
        }

        return application;
    }

    /// <summary>
    /// Inicializar Jobs recorrentes.
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public static IApplicationBuilder StartRecurrentJobs(this IApplicationBuilder application)
    {
        try
        {
            string EveryThreeMinutes = "0 */3 * ? * *";

            RecurringJob.AddOrUpdate<JobMethods>(
                        "resend-failed-mail-recurrent-job", jobMethods
                            => jobMethods.ResendFailedMailsAsync(), EveryThreeMinutes);
        }
        catch (Exception exception)
        {
            Log.Error($"[LOG ERROR] - Exception:{exception.Message} - {JsonConvert.SerializeObject(exception)}\n"); throw;
        }

        return application;
    }

    /// <summary>
    /// Retorna um provider do service.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static ServiceProvider GetProvider(this IServiceCollection services)
    {
        return services.BuildServiceProvider();
    }
}

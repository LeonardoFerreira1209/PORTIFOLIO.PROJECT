using APPLICATION.APPLICATION.CONFIGURATIONS;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.CUSTOMAUTHORIZE.FILTER;
using APPLICATION.DOMAIN.GRAPHQL.QUERY;
using APPLICATION.INFRAESTRUTURE.MIDDLEWARE;
using APPLICATION.INFRAESTRUTURE.SIGNALR;
using Hangfire;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using System;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var configurations = builder.Configuration;

    /// <sumary>
    /// Pega o appsettings baseado no ambiente em execução.
    /// </sumary>
    configurations
         .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables();

    /// <summary>
    /// Chamada das configurações do projeto.
    /// </summary>
    builder.Services
        .AddHttpContextAccessor()
            .Configure<AppSettings>(configurations)
                .AddSingleton<AppSettings>()
                    .AddEndpointsApiExplorer()
                        .AddOptions()
                            .ConfigureLanguage()
                                .ConfigureContexto(configurations)
                                    .ConfigureIdentityServer(configurations)
                                         .ConfigureAuthorization()
                                    .ConfigureAuthentication(configurations)
                                .ConfigureApllicationCookie()
                            .ConfigureSwagger(configurations)
                        .ConfigureDependencies(configurations, builder.Environment)
                    .ConfigureRefit(configurations)
                .AddGraphQLServer()
            .AddQueryType<UserQuery>()
        .SetPagingOptions(new PagingOptions
        {
            MaxPageSize = 10,
            IncludeTotalCount = true,
            DefaultPageSize = 10,

        }).AddProjections();

    if (
        builder.Environment.IsProduction()) {
        builder.Services
            .ConfigureTelemetry(configurations)
                .ConfigureApplicationInsights(configurations);
    }

    builder.Services
         .ConfigureSerilog(configurations)
            .ConfigureHangFire(configurations)
                .ConfigureFluentSchedulerJobs()
                    .ConfigureSubscribers()
                .ConfigureHealthChecks(configurations)
             .ConfigureCors()
        .AddControllers(options =>
        {
            options.EnableEndpointRouting = false;

            options.Filters.Add(new ProducesAttribute("application/json"));

        })
        .AddNewtonsoftJson(
            options 
            => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

    builder.Services.AddSignalR();

    var applicationbuilder = builder.Build();

    applicationbuilder.MapGraphQL();

    applicationbuilder
        .UseMiddleware<ErrorHandlerMiddleware>()
            .UseHttpsRedirection()
                .UseDefaultFiles()
                    .UseStaticFiles()
                        .UseCookiePolicy()
                             .UseRouting()
                                .UseCors("CorsPolicy")
                                    .UseResponseCaching()
                                .UseAuthorization()
                            .UseAuthentication()
                        .UseHealthChecks()
                    .UseSwaggerConfigurations(configurations)
                .UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization =
                        new[] { new CustomAuthorizeHangfireFilter() }
                })
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<HubNotifications>("/notifications");
                    endpoints.MapControllers();
                })
                .Seeds(applicationbuilder).Result.StartRecurrentJobs();

        applicationbuilder
            .Lifetime.ApplicationStarted
                .Register(() => Log.Debug(
                        $"[LOG DEBUG] - Aplicação inicializada com sucesso: [PORTIFOLIO.API]\n"));

    applicationbuilder.Run();
}
catch (Exception exception)
{
    Log.Error($"[LOG ERROR] - Ocorreu um erro ao inicializar a aplicacao [PORTIFOLIO.API] - {exception.Message}\n"); throw;
}
﻿namespace APPLICATION.DOMAIN.DTOS.CONFIGURATION;

/// <summary>
/// Classe responsavel por receber os dados do Appsettings.
/// </summary>
public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; }
    public SwaggerInfo SwaggerInfo { get; set; }
    public Configuracoes Configuracoes { get; set; }
    public RetryPolicy RetryPolicy { get; set; }
    public Auth Auth { get; set; }
    public AzureBlobStorage AzureBlobStorage { get; set; }
    public ServiceBus ServiceBus { get; set; }
    public Mail Mail { get; set; }
    public OpenAi OpenAi { get; set; }
}

/// <summary>
/// Classe responsável por receber dados de retry policy.
/// </summary>
public class RetryPolicy
{
    public string RetryOn { get; set; }
    public int RetryCount { get; set; }
    public int RetryEachSecond { get; set; }
}

/// <summary>
/// Classe de conexões.
/// </summary>
public class ConnectionStrings
{
    public string BaseDados { get; set; }
    public string ServiceBus { get; set; }
    public string AzureBlobStorage { get; set; }
}

/// <summary>
/// Classe de config do swagger.
/// </summary>
public class SwaggerInfo
{
    public string ApiDescription { get; set; }
    public string ApiVersion { get; set; }
    public string UriMyGit { get; set; }
}

/// <summary>
/// Classe de config diversas.
/// </summary>
public class Configuracoes
{
    public int TimeOutDefault { get; set; }
}

/// <summary>
/// Classe de config de autenticação.
/// </summary>
public class Auth
{
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    public string SecurityKey { get; set; }
    public int ExpiresIn { get; set; }
    public Password Password { get; set; }
}

/// <summary>
/// Classe de config do blob storage.
/// </summary>
public class AzureBlobStorage
{
    public string Container { get; set; }
}

/// <summary>
/// Classe de dados do serviceBus
/// </summary>
public class ServiceBus
{
    public int NumeroThreadsConsumer { get; set; }
    public string QueueEmail { get; set; }
    public string SubscriptionExemploName { get; set; }
    public int TempoReagendamentoMinutos { get; set; }
    public int QuantidadeMaximaDeRetentativas { get; set; }
}

/// <summary>
/// Classe de config de senha.
/// </summary>
public class Password
{
    public int RequiredLength { get; set; }
}

/// <summary>
/// Classe de config de e-mail
/// </summary>
public class Mail
{
    /// <summary>
    /// Chave da API.
    /// </summary>
    public string ApiKey { get; set; }
}

/// <summary>
/// Config da API do OpenAi.
/// </summary>
public class OpenAi
{
    /// <summary>
    /// Url base.
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// Chave de API.
    /// </summary>
    public string ApiKey { get; set; }
}
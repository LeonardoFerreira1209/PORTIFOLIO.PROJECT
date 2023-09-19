using APPLICATION.DOMAIN.CONTRACTS.FEATUREFLAGS;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.EXCEPTIONS;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PORTIFOLIO.API.CONTROLLER.BASE;

/// <summary>
/// Controller base
/// </summary>
public class BaseControllercs : ControllerBase
{
    private readonly IFeatureFlags _featureFlags;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="featureFlags"></param>
    /// <param name=""></param>
    public BaseControllercs(
        IFeatureFlags featureFlags)
    {
        _featureFlags = featureFlags;
    }

    /// <summary>
    /// Método que verifica se o endpoint está ativado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodName"></param>
    /// <param name="method"></param>
    /// <param name="methodDescription"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    protected async Task<T> ExecuteAsync<T>(string methodName, Func<Task<T>> method, string methodDescription)
    {
        var featureFlag = await _featureFlags.GetFeatureDefinitionAsync(methodName);

        if (featureFlag.IsEnabled)
            throw new CustomException(HttpStatusCode.NotImplemented, null, new List<DadosNotificacao> {
                new DadosNotificacao("Método inativado!")
            });

        return await Tracker.Time(() => method(), methodDescription);
    }
}

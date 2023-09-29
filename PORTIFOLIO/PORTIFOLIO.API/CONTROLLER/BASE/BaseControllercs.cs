using APPLICATION.DOMAIN.CONTRACTS.FEATUREFLAGS;
using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.BASE;
using APPLICATION.DOMAIN.DTOS.RESPONSE.BASE;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.EXCEPTIONS.BASE;
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
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="featureFlags"></param>
    /// <param name=""></param>
    public BaseControllercs(
        IFeatureFlags featureFlags, IUnitOfWork unitOfWork)
    {
        _featureFlags = featureFlags;
        _unitOfWork = unitOfWork;
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
        var featureFlag
            = await _featureFlags.GetFeatureDefinitionAsync(methodName)
            ?? await _featureFlags.CreateAsync(new FeatureFlags
            {
                Name = methodName,
                Created = DateTime.Now,
                IsEnabled = true,
                Status = Status.Active,
            }).ContinueWith(async (taskResult) =>
            {
                await _unitOfWork.CommitAsync();

                return taskResult.Result;

            }).Result;

        if (featureFlag.IsEnabled is false) 
            throw new CustomException(HttpStatusCode.NotImplemented, null, new List<DadosNotificacao> {
                new DadosNotificacao($"Método {methodName} inativado!")
            });

        return await Tracker.Time(
            () => method(), methodDescription);
    }
}

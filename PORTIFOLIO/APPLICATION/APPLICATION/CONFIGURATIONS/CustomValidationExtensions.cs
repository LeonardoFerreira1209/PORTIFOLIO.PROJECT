using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.EXCEPTIONS;
using FluentValidation.Results;
using System.Net;

namespace APPLICATION.APPLICATION.CONFIGURATIONS;

/// <summary>
/// Extensão para o Validation Customizados.
/// </summary>
public static class CustomValidationExtensions
{
    /// <summary>
    /// Tratamentos de erros.
    /// </summary>
    /// <param name="validationResult"></param>
    /// <returns></returns>
    public static Task GetValidationErrors(this ValidationResult validationResult, object dados = null)
    {
        var notificacoes = new List<DadosNotificacao>();

        foreach (var error in validationResult.Errors) notificacoes.Add(new DadosNotificacao(error.ErrorMessage));

        throw new CustomException(HttpStatusCode.BadRequest, dados, notificacoes);
    }
}

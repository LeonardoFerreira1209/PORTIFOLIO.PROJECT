using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.EXCEPTIONS.USER;
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
    public static Task CarregarErrosValidator(this ValidationResult validationResult, object dados = null)
    {
        var _notificacoes = new List<DadosNotificacao>();

        foreach (var erro in validationResult.Errors) _notificacoes.Add(new DadosNotificacao(erro.ErrorMessage));

        throw new CustomException(HttpStatusCode.BadRequest, dados, _notificacoes);
    }
}

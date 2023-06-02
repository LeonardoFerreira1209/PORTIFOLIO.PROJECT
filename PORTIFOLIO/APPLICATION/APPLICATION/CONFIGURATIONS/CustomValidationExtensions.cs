using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.UTILS;
using APPLICATION.DOMAIN.EXCEPTIONS.USER;
using APPLICATION.ENUMS;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace APPLICATION.APPLICATION.CONFIGURATIONS;

/// <summary>
/// Extensão para o Validation Customizados.
/// </summary>
[ExcludeFromCodeCoverage]
public static class CustomValidationExtensions
{
    /// <summary>
    /// Tratamentos de erros.
    /// </summary>
    /// <param name="validationResult"></param>
    /// <returns></returns>
    public static Task /* ObjectResult*/ CarregarErrosValidator(this ValidationResult validationResult, object dados = null)
    {
        var _notificacoes = new List<DadosNotificacao>();

        foreach (var erro in validationResult.Errors) _notificacoes.Add(new DadosNotificacao(erro.ErrorMessage));

        //return new BadRequestObjectResult(new ApiResponse<object>(false, StatusCodes.ErrorBadRequest, dados, _notificacoes));

        throw new InvalidCreateUserRequestException<UserCreateRequest>()
        {
            Object = new UserCreateRequest(),
            Reason = "Erro ao validar usuário.",
        };
    }
}

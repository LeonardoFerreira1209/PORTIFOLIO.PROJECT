using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using FluentValidation;

namespace APPLICATION.DOMAIN.VALIDATORS;

/// <summary>
/// Validator de login, verifica se os campos foram informados corretamente.
/// </summary>
public class RemoveClaimValidator : AbstractValidator<ClaimRequest>
{
    public RemoveClaimValidator()
    {
        RuleFor(claim => claim.Type).NotEmpty().NotNull().WithErrorCode(ErrorCode.CamposObrigatorios.ToCode()).WithMessage("Preencha o campo type."); 

        RuleFor(claim => claim.Value).NotEmpty().NotNull().WithErrorCode(ErrorCode.CamposObrigatorios.ToCode()).WithMessage("Preencha o campo value.");
    }
}

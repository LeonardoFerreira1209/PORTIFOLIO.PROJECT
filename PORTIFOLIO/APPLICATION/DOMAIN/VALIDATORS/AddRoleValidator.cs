using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.ENUMS;
using APPLICATION.DOMAIN.UTILS.EXTENSIONS;
using FluentValidation;

namespace APPLICATION.DOMAIN.VALIDATORS;

/// <summary>
/// Validator de login, verifica se os campos foram informados corretamente.
/// </summary>
public class AddRoleValidator : AbstractValidator<RoleRequest>
{
    public AddRoleValidator()
    {
        RuleFor(role => role.Name).NotEmpty().NotNull().WithErrorCode(ErrorCode.CamposObrigatorios.ToCode()).WithMessage("Preencha o campo Nome.");
    }
}

using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE;

/// <summary>
/// Classe de response para usuário.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Id do usuário.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Id do arquivo de imagem.
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// Dados do arquivo.
    /// </summary>
    public FileResponse File { get; set; }

    /// <summary>
    /// Nome de usuário.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Primeiro nome.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Ultimo nome.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Email.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Numero de telefone.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gênero.
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// RG.
    /// </summary>
    public string RG { get; set; }

    /// <summary>
    /// CPF.
    /// </summary>
    public string CPF { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? Updated { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public Status Status { get; set; }
}

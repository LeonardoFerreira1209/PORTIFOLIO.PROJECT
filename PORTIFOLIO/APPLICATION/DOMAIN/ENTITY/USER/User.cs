﻿using APPLICATION.DOMAIN.ENUMS;
using Microsoft.AspNetCore.Identity;

namespace APPLICATION.DOMAIN.ENTITY.USER;

public class User : IdentityUser<Guid>
{
    /// <summary>
    /// Primeiro nome.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Ultimo nome.
    /// </summary>
    public string LastName { get; set; }

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

    /// <summary>
    /// Id do arquivo de imagem de perfil.
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// Imagem de perfil.
    /// </summary>
    public virtual File File { get; set; }
}

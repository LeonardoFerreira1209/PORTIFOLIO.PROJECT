﻿using APPLICATION.DOMAIN.CONTRACTS.ENTITY;
using APPLICATION.ENUMS;
using Microsoft.AspNetCore.Identity;

namespace APPLICATION.DOMAIN.ENTITY.ROLE;

public class RoleEntity : IdentityRole<Guid>, IEntity
{
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

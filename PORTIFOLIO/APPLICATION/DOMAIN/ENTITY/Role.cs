﻿using APPLICATION.DOMAIN.ENUMS;
using Microsoft.AspNetCore.Identity;

namespace APPLICATION.DOMAIN.ENTITY;

public class Role : IdentityRole<Guid>
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

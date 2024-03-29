﻿using System.ComponentModel;

namespace APPLICATION.DOMAIN.ENUMS
{
    public enum Claims
    {
        [Description("Accesso á Usuários.")]
        User = 1,

        [Description("Accesso á Pessoas.")]
        Person = 2,

        [Description("Accesso á Claims.")]
        Claim = 3,

        [Description("Accesso á Roles.")]
        Role = 4,

        [Description("Accesso á Chats.")]
        Chat = 5,
    }
}

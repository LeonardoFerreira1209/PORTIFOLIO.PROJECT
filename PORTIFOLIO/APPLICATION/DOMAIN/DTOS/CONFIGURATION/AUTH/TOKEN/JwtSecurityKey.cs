﻿using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APPLICATION.DOMAIN.DTOS.CONFIGURATION.AUTH.TOKEN;

public static class JwtSecurityKey
{
    public static SymmetricSecurityKey Create(string secret) => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
}

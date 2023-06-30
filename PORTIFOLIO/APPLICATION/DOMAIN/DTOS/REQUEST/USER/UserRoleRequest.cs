namespace APPLICATION.DOMAIN.DTOS.REQUEST.USER;

/// <summary>
/// Classe de request de cadastro de role no usuário.
/// </summary>
public class UserRoleRequest
{
    /// <summary>
    /// ctor
    /// </summary>
    public UserRoleRequest(string username, string roleName)
    {
        Username = username;
        RoleName = roleName;
    }

    /// <summary>
    /// Nome do usuário
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Nome da role.
    /// </summary>
    public string RoleName { get; set; }
}

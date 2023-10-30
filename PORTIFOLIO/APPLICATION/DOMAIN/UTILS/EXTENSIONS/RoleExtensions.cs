using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

public static class RoleExtensions
{
    public static Role ToIdentityRole(this RoleRequest roleRequest)
    {
        return new Role
        {
            Name = roleRequest.Name,
            Created = DateTime.UtcNow,
            Status = Status.Active
        };
    }

    public static RolesResponse ToResponse(this Role role)
    {
        return new RolesResponse
        {
            Name = role.Name,
            Created = role.Created.ToLocalTime(),
            Updated = role.Updated?.ToLocalTime(),
            Status = role.Status,
        };
    }
}

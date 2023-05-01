using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER.ROLE;
using APPLICATION.DOMAIN.ENTITY.ROLE;
using APPLICATION.ENUMS;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

public static class RoleExtensions
{
    public static RoleEntity ToIdentityRole(this RoleRequest roleRequest)
    {
        return new RoleEntity
        {
            Name = roleRequest.Name,
            Created = DateTime.Now,
            Status = Status.Active
        };
    }

    public static RolesResponse ToResponse(this RoleEntity role)
    {
        return new RolesResponse
        {
            Name = role.Name,
            Created = role.Created,
            Updated = role.Updated,
            Status = role.Status,
        };
    }
}

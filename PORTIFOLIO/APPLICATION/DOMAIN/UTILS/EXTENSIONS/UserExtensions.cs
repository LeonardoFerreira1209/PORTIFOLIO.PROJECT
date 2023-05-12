using APPLICATION.DOMAIN.BUILDERS.USER;
using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE.USER;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

/// <summary>
/// Extensões de usuários.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Converte um user create para um userEntity.
    /// </summary>
    /// <param name="userRequest"></param>
    /// <returns></returns>
    public static UserEntity ToIdentityUser(this UserCreateRequest userRequest)
        => UserBuilder.BuildCreateUser(
            userRequest.FirstName, userRequest.LastName, userRequest.UserName, userRequest.Email,
            userRequest.CPF, userRequest.RG, userRequest.Gender, userRequest.PhoneNumber, userRequest.Password
            );

    /// <summary>
    /// Convert um user update para um userEntity.
    /// </summary>
    /// <param name="userUpdateRequest"></param>
    /// <returns></returns>
    public static UserEntity ToCompleteUserUpdateWithRequest(this UserUpdateRequest userUpdateRequest, UserEntity user)
        => UserBuilder.BuildCompleteUser(
            userUpdateRequest.FirstName, userUpdateRequest.LastName, userUpdateRequest.UserName, userUpdateRequest.Email,
            userUpdateRequest.CPF, userUpdateRequest.RG, userUpdateRequest.Gender, userUpdateRequest.PhoneNumber, user.Status, user.NormalizedEmail, user.NormalizedUserName,
            user.AccessFailedCount, user.ConcurrencyStamp, user.EmailConfirmed, user.LockoutEnabled, user.LockoutEnd,
            user.PasswordHash, user.PhoneNumberConfirmed, user.SecurityStamp, user.TwoFactorEnabled
            );

    /// <summary>
    /// Convert um userEntity para um user response.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static UserResponse ToResponse(this UserEntity user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CPF = user.CPF,
            RG = user.RG,
            Created = user.Created,
            Updated = user.Updated,
            Gender = user.Gender,
            Status = user.Status
        };
    }
}

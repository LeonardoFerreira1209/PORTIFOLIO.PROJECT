using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.ENUMS;

namespace APPLICATION.DOMAIN.BUILDERS.USER;

/// <summary>
/// Criador de usuários.
/// </summary>
public sealed class UserBuilder
{
    /// <summary>
    /// Ctor
    /// </summary>
    private UserBuilder() {

    }

    /// <summary>
    /// Criar usuário somente com valores básicos
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="cpf"></param>
    /// <param name="rg"></param>
    /// <param name="gender"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    public static UserEntity BuildCreateUserEntity(string firstName, string lastName, string userName,
       string email, string cpf, string rg, Gender gender, string phoneNumber, string passwordHash) =>
       new()
       {
           FirstName = firstName,
           LastName = lastName,
           UserName = userName,
           Email = email,
           CPF = cpf,
           RG = rg,
           Gender = gender,
           Created = DateTime.Now,
           PhoneNumber = phoneNumber,
           Status = Status.Active,
           PasswordHash = passwordHash
       };

    /// <summary>
    /// Criar usuário com todos os valores possiveis.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="cpf"></param>
    /// <param name="rg"></param>
    /// <param name="gender"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="normalizedEmail"></param>
    /// <param name="normalizedUserName"></param>
    /// <param name="accessFailedCount"></param>
    /// <param name="concurrencyStamp"></param>
    /// <param name="emailConfirmed"></param>
    /// <param name="lockoutEnabled"></param>
    /// <param name="lockoutEnd"></param>
    /// <param name="passwordHash"></param>
    /// <param name="phoneNumberConfirmed"></param>
    /// <param name="securityStamp"></param>
    /// <param name="twoFactorEnabled"></param>
    /// <returns></returns>
    public static UserEntity BuildCompleteUserEntity(string firstName, string lastName, string userName,
        string email, string cpf, string rg, Gender gender, string phoneNumber, Status status, string normalizedEmail, string normalizedUserName,
        int accessFailedCount, string concurrencyStamp, bool emailConfirmed, bool lockoutEnabled, 
        DateTimeOffset? lockoutEnd, string passwordHash, bool phoneNumberConfirmed, string securityStamp,
        bool twoFactorEnabled) =>
        new()
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email,
            CPF = cpf,
            RG = rg,
            Gender = gender,
            Created = DateTime.Now,
            PhoneNumber = phoneNumber,
            Status = status,
            NormalizedEmail = normalizedEmail,
            NormalizedUserName = normalizedUserName,
            AccessFailedCount = accessFailedCount,
            ConcurrencyStamp = concurrencyStamp,
            EmailConfirmed = emailConfirmed,
            LockoutEnabled = lockoutEnabled,
            LockoutEnd = lockoutEnd,
            PasswordHash = passwordHash,
            PhoneNumberConfirmed = phoneNumberConfirmed,
            SecurityStamp = securityStamp,
            TwoFactorEnabled = twoFactorEnabled
        };

    /// <summary>
    /// Converte os dados de user update request para userEntity.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <param name="userUpdateRequest"></param>
    /// <returns></returns>
    public static UserEntity BuilderUserEntityFromUserUpdateRequest(
        UserEntity userEntity, UserUpdateRequest userUpdateRequest)
    {
        userEntity.RG = userUpdateRequest.RG;
        userEntity.CPF = userUpdateRequest.CPF;
        userEntity.FirstName = userUpdateRequest.FirstName;
        userEntity.LastName = userUpdateRequest.LastName;
        userEntity.Status = userEntity.Status;
        userEntity.Created = userEntity.Created;
        userEntity.Updated = DateTime.Now;
        userEntity.Gender = userEntity.Gender;

        return userEntity;
    }
}

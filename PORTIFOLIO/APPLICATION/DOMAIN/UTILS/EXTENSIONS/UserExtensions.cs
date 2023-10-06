using APPLICATION.DOMAIN.BUILDERS.USER;
using APPLICATION.DOMAIN.DTOS.REQUEST.USER;
using APPLICATION.DOMAIN.DTOS.RESPONSE;
using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.ENUMS;

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
    public static User ToIdentityUser(this UserCreateRequest userRequest)
        => new UserEntityBuilder()
                .AddCredentials(userRequest.UserName, userRequest.Password)
                    .AddEmail(userRequest.Email)
                       .AddPhoneNumber(userRequest.PhoneNumber)
                          .AddDocuments(userRequest.RG, userRequest.CPF)
                             .AddCompleteName(userRequest.FirstName, userRequest.LastName)
                                .AddEmail(userRequest.Email)
                                   .AddGender(userRequest.Gender)
                                      .AddCreatedDate(DateTime.Now)
                                          .AddStatus(Status.Active)
                                              .Builder();

    /// <summary>
    /// Convert um user update para um userEntity.
    /// </summary>
    /// <param name="userUpdateRequest"></param>
    /// <returns></returns>
    public static User TransformUserEntityFromUserUpdateRequest(this User userEntity, UserUpdateRequest userUpdateRequest, bool emailConfirmed)
        => new UserEntityBuilder()
                .AddCredentials(userEntity.UserName, userEntity.PasswordHash)
                    .AddEmail(userEntity.Email)
                        .AddPhoneNumber(userEntity.PhoneNumber)
                            .AddDocuments(userUpdateRequest.RG, userUpdateRequest.CPF)
                               .AddCompleteName(userUpdateRequest.FirstName, userUpdateRequest.LastName)
                                  .AddEmail(userEntity.Email, emailConfirmed)
                                     .AddGender(userUpdateRequest.Gender)
                                        .AddCreatedDate(userEntity.Created)
                                           .AddUpdatedDate(DateTime.Now)
                                              .AddStatus(Status.Active)
                                                 .Builder(userEntity);

    /// <summary>
    /// Convert um userEntity para um user response.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FileId = user.FileId,
            File = user.File?.ToResponse(),
            UserName = user.UserName,
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

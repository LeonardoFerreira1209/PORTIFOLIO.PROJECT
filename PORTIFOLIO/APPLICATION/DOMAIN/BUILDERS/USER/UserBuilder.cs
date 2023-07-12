using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.BUILDERS.USER;

/// <summary>
/// UserEntityBuilder
/// </summary>
public sealed class UserEntityBuilder
{
    private string firstName, lastName, rg, cpf, email, phoneNumber, username, passwordHash;

    private bool emailConfirmed, phoneConfirmed;

    private Gender gender;

    private DateTime created, updated;

    private Status status;

    /// <summary>
    /// Adicionar nome completo do usuário.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns></returns>
    public UserEntityBuilder AddCompleteName(string firstName, string lastName)
    {
        this.firstName = firstName;
        this.lastName = lastName;

        return this;
    }

    /// <summary>
    /// Adicionar documentos.
    /// </summary>
    /// <param name="rg"></param>
    /// <param name="cpf"></param>
    /// <returns></returns>
    public UserEntityBuilder AddDocuments(string rg, string cpf)
    {
        this.rg = rg;
        this.cpf = cpf;

        return this;
    }

    /// <summary>
    /// Adicionar status.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public UserEntityBuilder AddStatus(Status status)
    {
        this.status = status;

        return this;
    }

    /// <summary>
    /// Adicionar dados de email.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="emailConfirmed"></param>
    /// <returns></returns>
    public UserEntityBuilder AddEmail(string email, bool emailConfirmed = false)
    {
        this.email = email;
        this.emailConfirmed = emailConfirmed;

        return this;
    }

    /// <summary>
    /// Adiconar nome de usuário & senha.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    public UserEntityBuilder AddCredentials(string username, string passwordHash)
    {
        this.username = username;
        this.passwordHash = passwordHash;

        return this;
    }

    /// <summary>
    /// Adicionar número de celular.
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="phoneConfirmed"></param>
    /// <returns></returns>
    public UserEntityBuilder AddPhoneNumber(string phoneNumber, bool phoneConfirmed = false)
    {
        this.phoneNumber = phoneNumber;
        this.phoneConfirmed = phoneConfirmed;

        return this;
    }

    /// <summary>
    /// Adicionar o sexo do usuário.
    /// </summary>
    /// <param name="gender"></param>
    /// <returns></returns>
    public UserEntityBuilder AddGender(Gender gender)
    {
        this.gender = gender;

        return this;
    }

    /// <summary>
    /// Adicionar data de criação.
    /// </summary>
    /// <param name="createdDate"></param>
    /// <returns></returns>
    public UserEntityBuilder AddCreatedDate(DateTime created)
    {
        this.created = created;

        return this;
    }

    /// <summary>
    /// Adicionar data de atualização.
    /// </summary>
    /// <param name="createdDate"></param>
    /// <returns></returns>
    public UserEntityBuilder AddUpdatedDate(DateTime updated)
    {
        this.updated = updated;

        return this;
    }

    /// <summary>
    /// Builder.
    /// </summary>
    /// <param name="userEntity"></param>
    /// <returns></returns>
    public UserEntity Builder(UserEntity userEntity = null)
    {
        userEntity ??= new UserEntity();

        userEntity.FirstName = firstName;
        userEntity.LastName = lastName;
        userEntity.UserName = username;
        userEntity.PasswordHash = passwordHash;
        userEntity.Email = email;
        userEntity.PhoneNumber = phoneNumber;
        userEntity.Gender = gender;
        userEntity.CPF = cpf;
        userEntity.RG = rg;
        userEntity.EmailConfirmed = emailConfirmed;
        userEntity.PhoneNumberConfirmed = phoneConfirmed;
        userEntity.Created = created;
        userEntity.Updated = updated;
        userEntity.Status = status;

        return userEntity;
    }
}

using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.APPLICATION.GRAPHQL.OBJECTTYPE;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Name("UserType").Description("Objeto que representa o usuário.");

        descriptor.Field(userEntity => userEntity.FirstName).Description("Nome do usuário.");
    }
}

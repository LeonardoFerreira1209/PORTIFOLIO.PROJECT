using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.GRAPHQL.OBJECTTYPE;

public class UserType : ObjectType<UserEntity>
{
    protected override void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
    {
        descriptor.Name("UserType").Description("Objeto que representa o usuário.");

        descriptor.Field(userEntity => userEntity.FirstName).Description("Nome do usuário.");
    }
}

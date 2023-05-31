using APPLICATION.DOMAIN.ENTITY.USER;
using APPLICATION.DOMAIN.GRAPHQL.DATALOADERS;

namespace APPLICATION.DOMAIN.GRAPHQL.QUERY;

public class UserQuery
{
    /// <summary>
    /// Retorna um usuário através do Id.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userBatchDataLoader"></param>
    /// <returns></returns>
    [UseProjection]
    public async Task<UserEntity> GetUserByIdAsync(
        Guid userId,
        UserBatchDataLoader userBatchDataLoader)
        => await userBatchDataLoader.LoadAsync(userId);

    /// <summary>
    /// Retorna usuários baseado no nome.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="usersBatchByNameDataloader"></param>
    /// <returns></returns>
    [UsePaging][UseProjection]
    public async Task<IEnumerable<UserEntity>> GetUsersByNameAsync(
        string name,
        UsersBatchByNameDataloader usersBatchByNameDataloader)
        => await usersBatchByNameDataloader.LoadAsync(name);
}

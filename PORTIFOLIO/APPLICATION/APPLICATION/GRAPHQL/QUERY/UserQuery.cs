using APPLICATION.APPLICATION.GRAPHQL.DATALOADERS;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.APPLICATION.GRAPHQL.QUERY;

/// <summary>
/// Query de usuários.
/// </summary>
public class UserQuery
{
    /// <summary>
    /// Retorna um usuário através do Id.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userBatchDataLoader"></param>
    /// <returns></returns>
    [UseProjection]
    public async Task<User> GetUserByIdAsync(
        Guid userId,
        UserBatchDataLoader userBatchDataLoader)
        => await userBatchDataLoader.LoadAsync(userId);

    /// <summary>
    /// Retorna usuários baseado no nome.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="usersBatchByNameDataloader"></param>
    /// <returns></returns>
    [UsePaging]
    [UseProjection]
    public async Task<IEnumerable<User>> GetUsersByNameAsync(
        string name,
        UsersBatchByNameDataloader usersBatchByNameDataloader)
        => await usersBatchByNameDataloader.LoadAsync(name);
}

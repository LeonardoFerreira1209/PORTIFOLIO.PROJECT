using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.USER;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.GRAPHQL.DATALOADERS;

public class UserBatchDataLoader : BatchDataLoader<Guid, User>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="batchScheduler"></param>
    /// <param name="options"></param>
    public UserBatchDataLoader(
        IUserRepository userRepository,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options)
        : base(batchScheduler, options)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Retorna um usuário por Id.
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<IReadOnlyDictionary<Guid, User>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetByIdsAsync(keys.ToList());
        return users.ToDictionary(user => user.Id);
    }
}

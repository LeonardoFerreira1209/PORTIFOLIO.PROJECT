using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY.USER;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.GRAPHQL.DATALOADERS;

public class UsersBatchByNameDataloader
    : GroupedDataLoader<string, User>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="batchScheduler"></param>
    /// <param name="options"></param>
    public UsersBatchByNameDataloader(
        IUserRepository userRepository,
        IBatchScheduler batchScheduler,
        DataLoaderOptions options
        ) : base(batchScheduler, options)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Retorna uma lista de usuários por nome.
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<ILookup<string, User>> LoadGroupedBatchAsync(
        IReadOnlyList<string> keys, 
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetByNamesAsync(keys.ToList());
        return users.ToLookup(user => user.FirstName);
    }
}

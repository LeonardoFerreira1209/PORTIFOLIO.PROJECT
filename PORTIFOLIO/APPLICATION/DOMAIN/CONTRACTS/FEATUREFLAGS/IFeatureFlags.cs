using APPLICATION.DOMAIN.CONTRACTS.REPOSITORY;
using APPLICATION.DOMAIN.ENTITY;

namespace APPLICATION.DOMAIN.CONTRACTS.FEATUREFLAGS;

/// <summary>
/// Interface de feature flags
/// </summary>
public interface IFeatureFlags : IGenerictEntityCoreRepository<FeatureFlags>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="featureName"></param>
    /// <returns></returns>
    Task<FeatureFlags> GetFeatureDefinitionAsync(string featureName);

    /// <summary>
    /// Retonrna todas as featuire flags.
    /// </summary>
    /// <returns></returns>
    IAsyncEnumerable<FeatureFlags> GetAllFeatureDefinitionsAsync();
}

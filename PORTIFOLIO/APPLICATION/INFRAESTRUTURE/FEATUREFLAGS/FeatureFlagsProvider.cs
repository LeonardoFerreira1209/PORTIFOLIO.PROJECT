using APPLICATION.DOMAIN.CONTRACTS.FEATUREFLAGS;
using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.INFRAESTRUTURE.CONTEXTO;
using APPLICATION.INFRAESTRUTURE.REPOSITORY;
using Microsoft.EntityFrameworkCore;

namespace APPLICATION.INFRAESTRUTURE.FEATUREFLAGS;

/// <summary>
/// Feature flags provider
/// </summary>
public class FeatureFlagsProvider : GenericEntityCoreRepository<FeatureFlags>, IFeatureFlags
{
    private readonly Context _context;

    /// <summary>
    /// ctor
    /// </summary>
    public FeatureFlagsProvider(
        Context context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Recupera uma feature flag
    /// </summary>
    /// <param name="featureName"></param>
    /// <returns></returns>
    public async Task<FeatureFlags> GetFeatureDefinitionAsync(string featureName) 
        => await _context.FeatureFlags.SingleOrDefaultAsync(f => f.Name == featureName);
    
    /// <summary>
    /// Retonrna todas as featuire flags.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<FeatureFlags> GetAllFeatureDefinitionsAsync() 
        => _context.FeatureFlags.AsAsyncEnumerable();
}

using Polly;

namespace APPLICATION.DOMAIN.UTILS.EXTENSIONS;

/// <summary>
/// Classe de retentativas.
/// </summary>
public static class RetryPolicy
{
    /// <summary>
    /// Executa retentativas para métodos Task.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="retryCount"></param>
    /// <returns></returns>
    public static async Task ExecuteAsync(Func<Task> method, int retryCount)
    {
        var retryPolicy =
            Policy.Handle<Exception>()
                .RetryAsync(retryCount);

        await retryPolicy.ExecuteAsync(async ()
            => await method());
    }

    /// <summary>
    /// Executa retentativas parta métodos Task com retorno.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="method"></param>
    /// <param name="retryCount"></param>
    /// <returns></returns>
    public static async Task<T> ExecuteAsync<T>(this Func<Task<T>> method, int retryCount)
    {
        var retryPolicy =
            Policy.Handle<Exception>()
                .RetryAsync(retryCount);

        var result = await retryPolicy.ExecuteAsync(async ()
            => await method());

        return result;
    }
}

using APPLICATION.DOMAIN.DTOS.CONFIGURATION;
using APPLICATION.INFRAESTRUTURE.JOBS.INTERFACES.RECURRENT;
using Microsoft.Extensions.Options;
using Serilog;

namespace APPLICATION.INFRAESTRUTURE.JOBS.RECURRENT;

public class ProcessResendFailedMailsJob : IResendFailedMailsJob
{
    private readonly bool _execute = true;

    private readonly IOptions<AppSettings> _configuracoes;

    public ProcessResendFailedMailsJob(IOptions<AppSettings> configuracoes)
    {
        _configuracoes = configuracoes;
    }

    public void Execute()
    {
        ResendFailedMailsJob().Wait();
    }

    public async Task ResendFailedMailsJob()
    {
        try
        {
            if (_execute)
            {
                Log.Information($"[LOG INFORMATION] - Processando Job de Delete de usuários sem pessaos vinculadas.\n");

                //await new UserRepository(_configuracoes).DeleteUserWithoutPerson();

                Log.Information("[LOG INFORMATION] - Finalizando Job de Delete de usuários sem pessaos vinculadas\n");
            }
            else
            {
                Log.Warning("[LOG WARNING] - Toggle para processar Job de Delete de usuários sem pessaos vinculadas desativada - {0}.\n", _execute);
            }
        }
        catch (Exception exception)
        {
            Log.Error("[LOG ERRO] - Erro ao processar Job - ProcessFailedPersonCreateJob - ({0}).\n", exception.Message);
        }
    }
}

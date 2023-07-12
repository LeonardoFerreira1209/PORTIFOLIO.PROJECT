using System.ComponentModel;

namespace APPLICATION.DOMAIN.ENUMS;

public enum ErrorCode
{
    [Description("Desculpe, tivemos um problema ao processar essa requisição.")]
    ErroInesperado = 500,

    [Description("Campos obrigatórios não informado.")]
    CamposObrigatorios = 1,
}

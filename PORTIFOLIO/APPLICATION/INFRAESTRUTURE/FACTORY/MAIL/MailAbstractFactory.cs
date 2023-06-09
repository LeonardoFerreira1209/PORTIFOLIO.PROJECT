﻿using APPLICATION.DOMAIN.CONTRACTS.SERVICES.MAIL;
using APPLICATION.DOMAIN.DTOS.MAIL.REQUEST;

namespace APPLICATION.INFRAESTRUTURE.FACTORY;

/// <summary>
/// Abstract Factory de e-mails.
/// </summary>
public abstract class MailAbstractFactory
{
    /// <summary>
    /// Assinatura abstrata para criação de serviços de e-mails.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <returns></returns>
    public abstract IMailService<TRequest, TResponse> CreateMailService<TRequest, TResponse>()
        where TRequest : MailRequestBase where TResponse : class;
}

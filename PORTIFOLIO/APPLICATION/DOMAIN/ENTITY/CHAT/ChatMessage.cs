﻿using APPLICATION.DOMAIN.ENTITY.BASE;
using APPLICATION.DOMAIN.ENTITY.USER;

namespace APPLICATION.DOMAIN.ENTITY.CHAT;

/// <summary>
/// Entidade de mensagens do chat.
/// </summary>
public class ChatMessage : Entity
{
    /// <summary>
    /// Id do usuário que enviou a mensagem.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Dados do usuário.
    /// </summary>
    public virtual User UserToSendMessage { get; set; }

    /// <summary>
    /// Id do chat em que a mensagem está vinculada.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Dados do chat em que a mensagem está vinculada.
    /// </summary>
    public virtual Chat Chat { get; set; }

    /// <summary>
    /// Mensagem
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Se a mensagem tem um commando.
    /// </summary>
    public bool HasCommand { get; set; }

    /// <summary>
    /// Comandos de interação.
    /// </summary>
    public string Command { get; set; }

    /// <summary>
    /// A mensagem foi gerada por um chatBot. 
    /// </summary>
    public bool IsChatBot { get; set; }

    /// <summary>
    /// É uma mensagem. 
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    /// Id do arquivo.
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// Arquivo
    /// </summary>
    public virtual File File { get; set; }
}

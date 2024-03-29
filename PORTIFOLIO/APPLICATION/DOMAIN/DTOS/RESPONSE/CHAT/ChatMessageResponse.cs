﻿using APPLICATION.DOMAIN.ENUMS;

namespace APPLICATION.DOMAIN.DTOS.RESPONSE.CHAT;

/// <summary>
/// Classe de response de chat messages.
/// </summary>
public class ChatMessageResponse
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime? Updated { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public virtual Status Status { get; set; }

    /// <summary>
    /// Id do usuário que enviou a mensagem.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Dados do usuário.
    /// </summary>
    public UserResponse UserToSendMessage { get; set; }

    /// <summary>
    /// Id do chat em que a mensagem está vinculada.
    /// </summary>
    public Guid ChatId { get; set; }

    /// <summary>
    /// Dados do chat em que a mensagem está vinculada.
    /// </summary>
    public ChatResponse Chat { get; set; }

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
    public FileResponse File { get; set; }
}

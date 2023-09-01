﻿using APPLICATION.DOMAIN.DTOS.CHAT;
using Microsoft.AspNetCore.Mvc;

namespace APPLICATION.DOMAIN.CONTRACTS.SERVICES.CHAT;

/// <summary>
/// Interface de ChatService
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Método responsável por criar um chat.
    /// </summary>
    /// <param name="chatRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> CreateChatAsync(ChatRequest chatRequest);

    /// <summary>
    /// Método responsável por criar e enviar uma mensagem.
    /// </summary>
    /// <param name="chatMessageRequest"></param>
    /// <returns></returns>
    Task<ObjectResult> SendMessageAsync(ChatMessageRequest chatMessageRequest);

    /// <summary>
    /// Métodos responsável por recuperar Chats do usuário.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ObjectResult> GetChatsByUserAsync(Guid userId);
}

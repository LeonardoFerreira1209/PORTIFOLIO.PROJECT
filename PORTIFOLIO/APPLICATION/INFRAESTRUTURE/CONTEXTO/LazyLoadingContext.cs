using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.ENTITY.USER;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = APPLICATION.DOMAIN.ENTITY.File;

namespace APPLICATION.INFRAESTRUTURE.CONTEXTO;

/// <summary>
/// Contexto de carregamento lento.
/// </summary>
public class LazyLoadingContext : IdentityDbContext<User, Role, Guid>
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="options"></param>
    public LazyLoadingContext(
        DbContextOptions<LazyLoadingContext> options) : base(options) { }

    /// <summary>
    /// Tabela de Chat.
    /// </summary>
    public DbSet<Chat> Chats { get; set; }

    /// <summary>
    /// Tabela de mensagens do chat.
    /// </summary>
    public DbSet<ChatMessage> ChatMessages { get; set; }

    /// <summary>
    /// Tabela de Eventos.
    /// </summary>
    public DbSet<Event> Events { get; set; }

    /// <summary>
    /// Tabela de Arquivos.
    /// </summary>
    public DbSet<File> Files { get; set; }

    /// <summary>
    /// Tabela de códigos de confirmação de usuários.
    /// 
    /// </summary>
    public DbSet<UserCode> AspNetUserCodes { get; set; }
}

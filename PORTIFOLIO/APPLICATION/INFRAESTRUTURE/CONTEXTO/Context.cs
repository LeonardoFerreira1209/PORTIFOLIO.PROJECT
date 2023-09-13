using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENTITY.CHAT;
using APPLICATION.DOMAIN.ENTITY.ROLE;
using APPLICATION.DOMAIN.ENTITY.USER;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APPLICATION.INFRAESTRUTURE.CONTEXTO;

/// <summary>
/// Classe de configuração do banco de dados.
/// </summary>
public class Context : IdentityDbContext<User, Role, Guid>
{
    public Context(
        DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

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
    /// Tabela de códigos de confirmação de usuários.
    /// 
    /// </summary>
    public DbSet<UserCode> AspNetUserCodes { get; set; }

    /// <summary>
    /// Configrações dos datatypes.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>()
            .HasOne(chat => chat.FirstUser)
            .WithMany()
            .HasForeignKey(chat => chat.FirstUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Chat>()
            .HasOne(chat => chat.SecondUser)
            .WithMany()
            .HasForeignKey(chat => chat.SecondUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ChatMessage>()
           .HasOne(m => m.UserToSendMessage)
           .WithMany()
           .HasForeignKey(m => m.UserId);

        base.OnModelCreating(modelBuilder);
    }
}

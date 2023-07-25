using APPLICATION.DOMAIN.ENTITY;
using APPLICATION.DOMAIN.ENTITY.ROLE;
using APPLICATION.DOMAIN.ENTITY.USER;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APPLICATION.INFRAESTRUTURE.CONTEXTO;

/// <summary>
/// Classe de configuração do banco de dados.
/// </summary>
public class Context : IdentityDbContext<UserEntity, RoleEntity, Guid>
{
    public Context(
        DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
    }

    /// <summary>
    /// Tabela de Eventos.
    /// </summary>
    public DbSet<EventEntity> Events { get; set; }

    /// <summary>
    /// Tabela de códigos de confirmação de usuários.
    /// 
    /// </summary>
    public DbSet<UserCodeEntity> AspNetUserCodes { get; set; }

    /// <summary>
    /// Configrações fos datatypes.
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

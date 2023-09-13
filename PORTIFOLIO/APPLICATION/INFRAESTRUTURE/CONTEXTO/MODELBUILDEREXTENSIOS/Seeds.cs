using Microsoft.EntityFrameworkCore;

namespace APPLICATION.INFRAESTRUTURE.CONTEXTO.MODELBUILDEREXTENSIOS;

public static class Seeds
{
    public static ModelBuilder SeedsInitial(this ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<UserEntity>().HasData(
        //    new UserEntity
        //    {
        //        FirstName = "Hyper",
        //        LastName = "Teste",
        //        Email = "Hyper.ip@outlook.com",
        //        EmailConfirmed = true,
        //        UserName = "User.Teste",
        //        Created = DateTime.Now,
        //        Status = Status.Active,
        //        PasswordHash = "Teste@123456"
        //    });

        //modelBuilder.Entity<RoleEntity>().HasData(
        //    new RoleEntity
        //    {
        //        Name = "administrator",
        //        Status = Status.Active,
        //        Created = DateTime.Now
        //    });

        //modelBuilder.Entity<Claims>().HasData(

        //    )

        //// outras entidades e dados iniciais

        //// adicione outras chamadas para modelBuilder.Entity<T>().HasData()
        //// conforme necessário para outras entidades e dados iniciais

        //// Exemplo de dados iniciais para Claims
        //modelBuilder.Entity<ClaimEntity>().HasData(
        //    new ClaimEntity
        //    {
        //        // dados da claim
        //    });

        return modelBuilder;
    }
}

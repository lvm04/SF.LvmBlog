using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SF.BlogData.Models;

namespace SF.BlogData
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }      // отношение между пользователями и ролями будет "многие ко многим"
        public DbSet<Role> Roles { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var role1 = new Role { Id = 1, Name = "admin" };
            var role2 = new Role { Id = 2, Name = "user" };
            var role3 = new Role { Id = 3, Name = "moderator" };

            modelBuilder.Entity<Role>().HasData(role1, role2, role3);

            modelBuilder.Entity<User>().HasData(
                    new User { Id = 1, Login = "admin", Name = "Администратор", Password = "123", Email = "admin@mail.ru" },
                    new User { Id = 2, Login = "user", Name = "Пользователь", Password = "123", Email = "user@mail.ru" },
                    new User { Id = 3, Login = "moder", Name = "Модератор", Password = "123", Email = "moder@mail.ru" },
                    new User { Id = 4, Login = "expert", Name = "Опытный", Password = "123", Email = "expert@mail.ru" }
            );

            // Имена таблиц в единственном числе
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.DisplayName());
            }
        }

    }


    // dotnet-ef migrations add [name]
    // dotnet-ef database update
}
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Entities;

namespace WebApplication.Infrastructure.Contexts
{
    public class InMemoryContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<ContactDetail> ContactDetails => Set<ContactDetail>();

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryDb");

            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasData(
                            new List<User>
                            {
                                new User { Id = 1, GivenNames = "Laura", LastName = "Bailey" },
                                new User { Id = 2, GivenNames = "Travis", LastName = "Willingham" },
                                new User { Id = 3, GivenNames = "Brian", LastName = "Foster" },
                                new User { Id = 4, GivenNames = "Matthew", LastName = "Mercer" },
                                new User { Id = 5, GivenNames = "Marisha", LastName = "Ray" },
                                new User { Id = 6, GivenNames = "Liam", LastName = "O'Brian" },
                                new User { Id = 7, GivenNames = "Taliesin", LastName = "Jaffe" },
                                new User { Id = 8, GivenNames = "Ashley", LastName = "Johnson" },
                                new User { Id = 9, GivenNames = "Sam", LastName = "Riegel" },
                                new User { Id = 10, GivenNames = "John", LastName = "Smith" },
                                new User { Id = 11, GivenNames = "Jane", LastName = "Smith" },
                                new User { Id = 12, GivenNames = "Linus", LastName = "Sebastian" },
                                new User { Id = 13, GivenNames = "Will", LastName = "Friedle" },
                            }
                        );

            modelBuilder.Entity<ContactDetail>()
                        .HasData(
                            new List<ContactDetail>
                            {
                                new ContactDetail
                                {
                                    Id = 1,
                                    EmailAddress = "john.smith@example.com",
                                    MobileNumber = "0411112222",
                                    UserId = 10
                                },
                                new ContactDetail
                                {
                                    Id = 2,
                                    EmailAddress = "jane.smith@example.com",
                                    MobileNumber = "0422223333",
                                    UserId = 11
                                }
                            }
                        );

            base.OnModelCreating(modelBuilder);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.Infrastructure.Contexts;
using WebApplication.Infrastructure.Entities;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly InMemoryContext _dbContext;

        public UserService(InMemoryContext dbContext)
        {
            _dbContext = dbContext;

            // this is a hack to seed data into the in memory database. Do not use this in production.
            _dbContext.Database.EnsureCreated();
        }

        /// <inheritdoc />
        public async Task<User?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            User? user = await _dbContext.Users.Where(user => user.Id == id)
                                         .Include(x => x.ContactDetail)
                                         .FirstOrDefaultAsync(cancellationToken);

            return user;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default)
        {
            // throw new NotImplementedException("Implement a way to find users that match the provided given names OR last name.");
            List<User> users = await _dbContext.Users.Where(user => user.GivenNames == givenNames || user.LastName == lastName)
                                         .Include(x => x.ContactDetail)
                                        .ToListAsync(cancellationToken);

            return users;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            // throw new NotImplementedException("Implement a way to get a 'page' of users.");
            List<User> users = await _dbContext.Users.Include(x => x.ContactDetail)
                                                     .ToListAsync(cancellationToken);

            // Calculate the starting index of the page
            int startIndex = count * (page - 1);

            // Calculate the end index of the page
            int endIndex = startIndex + count;

            // Make sure the end index doesn't exceed the list size
            if (endIndex > users.Count)
            {
                endIndex = users.Count;
            }

            // Return the page of items from the list
            return users.GetRange(startIndex, endIndex - startIndex);
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            var newUser = await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return newUser.Entity;
            //throw new NotImplementedException("Implement a way to add a new user, including their contact details.");
        }

        /// <inheritdoc />
        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var newUser = _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return newUser.Entity;
            // throw new NotImplementedException("Implement a way to update an existing user, including their contact details.");
        }

        /// <inheritdoc />
        public async Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            User? user = await _dbContext.Users.Where(user => user.Id == id)
                                         .Include(x => x.ContactDetail)
                                         .FirstOrDefaultAsync(cancellationToken);

            var deletedUser = _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();

            return deletedUser.Entity as User;
            // throw new NotImplementedException("Implement a way to delete an existing user, including their contact details.");
        }

        /// <inheritdoc />
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            // throw new NotImplementedException("Implement a way to count the number of users in the database.");
            List<User> users = await _dbContext.Users.Include(x => x.ContactDetail)
                                                     .ToListAsync(cancellationToken);
            return users.Count;
        }
    }
}

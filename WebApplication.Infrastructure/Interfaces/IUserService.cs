using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApplication.Infrastructure.Entities;

namespace WebApplication.Infrastructure.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Gets a user from the database.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The user.</returns>
        Task<User?> GetAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds users with the provided given names and last name.
        /// </summary>
        /// <param name="givenNames">The given names.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The list of matching users.</returns>
        Task<IEnumerable<User>> FindAsync(string? givenNames, string? lastName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a page of users.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="count">The number of users per page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The users.</returns>
        Task<IEnumerable<User>> GetPaginatedAsync(int page, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The added user.</returns>
        Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated user.</returns>
        Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an existing user in the database.
        /// </summary>
        /// <param name="id">The id of the user to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The deleted user.</returns>
        Task<User?> DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists the number of users in the database.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of users in the db.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// Interface of nuget users repository.
	/// </summary>
	public interface INugetUserRepository
	{
		/// <summary>
		/// Looking for user by apiKey.
		/// </summary>
		/// <param name="apiKey">ApiKey of user.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>User's name - if found, or null.</returns>
		Task<string> FindUserByApiKeyAsync(string apiKey, CancellationToken token);


		/// <summary>
		/// Creates a new user in system.
		/// </summary>
		/// <param name="apiKey">User's api key.</param>
		/// <param name="userName">User's name.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task CreateUserAsync(string apiKey, string userName, CancellationToken token);


		/// <summary>
		/// Updates user's name by api key.
		/// </summary>
		/// <param name="apiKey">User's api key.</param>
		/// <param name="userName">New user's name.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task UpdateUserAsync(string apiKey, string userName, CancellationToken token);


		/// <summary>
		/// Deletes user by api key.
		/// </summary>
		/// <param name="apiKey">User's api key.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task DeleteUserAsync(string apiKey, CancellationToken token);
	}
}

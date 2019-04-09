using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// The interface of nuget users repository, for writing.
	/// </summary>
	public interface INugetUserRepositoryWriter
	{	
		/// <summary>
		/// Creates a new user in system.
		/// </summary>
		/// <param name="apiKey">User's api key.</param>
		/// <param name="userName">User's name.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task CreateUserAsync(string apiKey, string userName, CancellationToken token);


		/// <summary>
		/// Updates user's name with api key.
		/// </summary>
		/// <param name="apiKey">User's api key.</param>
		/// <param name="userName">New user's name.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task UpdateUserAsync(string apiKey, string userName, CancellationToken token);


		/// <summary>
		/// Deletes user with api key.
		/// </summary>
		/// <param name="apiKey">User's api key.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		Task DeleteUserAsync(string apiKey, CancellationToken token);
	}
}

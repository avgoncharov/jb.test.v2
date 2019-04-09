using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// The interface of nuget users repository, read only.
	/// </summary>
	public interface INugetUserRepositoryReader
	{
		/// <summary>
		/// Lookings for user by apiKey.
		/// </summary>
		/// <param name="apiKey">ApiKey of user.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>User's name - if found, or null.</returns>
		Task<INugetUser> FindUserByApiKeyAsync(string apiKey, CancellationToken token);


		/// <summary>
		/// Lookings for user by user's name.
		/// </summary>
		/// <param name="name">User's name.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns>User's api key- if found, or null.</returns>
		Task<INugetUser> FindUserByNameAsync(string name, CancellationToken token);
	}
}

using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Interfaces;
using Serilog;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	public sealed class NugetUserRepositoryReader: INugetUserRepositoryReader
	{
		private readonly ILogger _logger = Log.Logger.ForContext<NugetUserRepositoryReader>();

		/// <inheritdoc/>
		public async Task<INugetUser> FindUserByApiKeyAsync(string apiKey, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof(apiKey));
			}

			using(var innerStore = new NugetStore())
			{
				var user = await innerStore.Users.FirstOrDefaultAsync(itr => itr.ApiKey == apiKey, token);

				return user != null ? new NugetUser(user.ApiKey, user.Name) : null;
			}
		}


		/// <inheritdoc/>
		public async Task<INugetUser> FindUserByNameAsync(string name, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException(nameof(name));
			}

			using(var innerStore = new NugetStore())
			{
				var user = await innerStore.Users.FirstOrDefaultAsync(itr => itr.Name == name, token);

				return user != null ? new NugetUser(user.ApiKey, user.Name) : null;
			}
		}

	}
}

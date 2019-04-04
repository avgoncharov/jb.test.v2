using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Implementation.DB.DTOs;
using JB.Test.V2.DAL.Interfaces;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	public sealed class NugetUserRepository : INugetUserRepository
	{
		private readonly NugetStore _store;


		public NugetUserRepository()
		{
			_store = new NugetStore();
		}


		public Task CreateUserAsync(string apiKey, string userName, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException(nameof(apiKey));

			if(string.IsNullOrWhiteSpace(userName))
				throw new ArgumentException(nameof(userName));

			_store.Users.Add(new NugetUserDto { ApiKey = apiKey, Name = userName });
			return _store.SaveChangesAsync(token);
		}


		public async Task DeleteUserAsync(string apiKey, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException(nameof(apiKey));

			var user = await _store.Users.FirstOrDefaultAsync(itr => itr.ApiKey == apiKey);
			if(user == null)
				return;

			_store.Users.Remove(user);
			await _store.SaveChangesAsync(token);
		}


		public async Task<string> FindUserByApiKeyAsync(string apiKey, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException(nameof(apiKey));

			var user = await _store.Users.FirstOrDefaultAsync(itr => itr.ApiKey == apiKey);
			return user?.Name;
		}


		public async Task UpdateUserAsync(string apiKey, string userName, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentException(nameof(apiKey));

			if(string.IsNullOrWhiteSpace(userName))
				throw new ArgumentException(nameof(userName));

			var user = await _store.Users.FirstOrDefaultAsync(itr => itr.ApiKey == apiKey);

			if(user == null)
				return;

			user.Name = userName;
			await _store.SaveChangesAsync(token);
		}
	}
}												       
using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Implementation.DB.DTOs;
using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using Serilog;
using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.DAL.Implementation
{
	public sealed class NugetUserRepositoryWriter : INugetUserRepositoryWriter
	{
		private readonly ILogger _logger = Log.Logger.ForContext<NugetUserRepositoryWriter>();
		private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
		private readonly NugetStore _store;


		public NugetUserRepositoryWriter()
		{
			_store = new NugetStore();
		}


		/// <inheritdoc/>
		public async Task CreateUserAsync(string apiKey, string userName, CancellationToken token)
		{
			await _semaphoreSlim.WaitAsync(token);
			try
			{
				if(string.IsNullOrWhiteSpace(apiKey))
				{
					throw new ArgumentException(nameof(apiKey));
				}

				if(string.IsNullOrWhiteSpace(userName))
				{
					throw new ArgumentException(nameof(userName));
				}

				if(await _store.Users.AnyAsync(itr => itr.ApiKey == apiKey, token))
				{
					throw new UserIsAlreadyExistException($"User with api key '{apiKey}' is already exist in system.");
				}

				_store.Users.Add(new NugetUserDto { ApiKey = apiKey, Name = userName });
				await _store.SaveChangesAsync(token);
			}
			catch(Exception ex)
			{
				_logger.Warning($"Can't create new user, reason: {ex}");
				throw;
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}


		/// <inheritdoc/>
		public async Task DeleteUserAsync(string apiKey, CancellationToken token)
		{
			await _semaphoreSlim.WaitAsync(token);
			try
			{
				if(string.IsNullOrWhiteSpace(apiKey))
				{
					throw new ArgumentException(nameof(apiKey));
				}

				var user = await _store.Users.FirstOrDefaultAsync(itr => itr.ApiKey == apiKey, token);
				if(user == null)
				{
					return;
				}

				_store.Users.Remove(user);
				await _store.SaveChangesAsync(token);
			}
			catch(Exception ex)
			{
				_logger.Warning($"Can't delete user with api key '{apiKey}', reason: {ex}");
				throw;
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}
		

		/// <inheritdoc/>
		public async Task UpdateUserAsync(string apiKey, string userName, CancellationToken token)
		{
			await _semaphoreSlim.WaitAsync(token);
			try
			{
				if(string.IsNullOrWhiteSpace(apiKey))
				{
					throw new ArgumentException(nameof(apiKey));
				}

				if(string.IsNullOrWhiteSpace(userName))
				{
					throw new ArgumentException(nameof(userName));
				}

				if((await _store.Users.AnyAsync(itr => itr.ApiKey == apiKey, token)) != true)
				{
					throw new UserNotFoundException($"User with api key '{apiKey}' not found.");
				}

				var user = await _store.Users.FirstAsync(itr => itr.ApiKey == apiKey, token);

				user.Name = userName;
				await _store.SaveChangesAsync(token);
			}
			catch(Exception ex)
			{
				_logger.Warning($"Can't udate user with api key '{apiKey}', reason: {ex}");
				throw;
			}
			finally
			{
				_semaphoreSlim.Release();
			}
		}
	}
}
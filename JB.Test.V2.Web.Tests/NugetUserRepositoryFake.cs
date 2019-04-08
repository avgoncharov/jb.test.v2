using JB.Test.V2.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JB.Test.V2.Web.Tests
{
	internal sealed class NugetUserRepositoryFake : INugetUserRepositoryReader, INugetUserRepositoryWriter
	{			
		private sealed class NugetUser : INugetUser
		{
			public string ApiKey { get; set; }

			public string Name { get; set; }
		}

		private readonly object _lockObj = new object();
		private readonly List<INugetUser> _users = new List<INugetUser>();

		internal static readonly string UserName = "Jone";
		internal static readonly string ApiKey = "Jone-Dow";

		Task INugetUserRepositoryWriter.CreateUserAsync(string apiKey, string userName, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof(apiKey));
			}

			if(string.IsNullOrWhiteSpace(userName))
			{
				throw new ArgumentException(nameof(userName));
			}

			lock(_lockObj)
			{
				if(_users.Any(itr => itr.ApiKey == apiKey))
					throw new DAL.Interfaces.Exceptions.UserIsAlreadyExistException();

				_users.Add(new NugetUser { ApiKey = apiKey, Name = userName });
			}

			return Task.CompletedTask;
		}

		Task INugetUserRepositoryWriter.DeleteUserAsync(string apiKey, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		Task<INugetUser> INugetUserRepositoryReader.FindUserByApiKeyAsync(string apiKey, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(apiKey))
			{
				throw new ArgumentException(nameof(apiKey));
			}

			lock(_lockObj)
			{
				return Task.FromResult(_users.FirstOrDefault(itr => itr.ApiKey == apiKey));
			}
		}

		async Task<INugetUser> INugetUserRepositoryReader.FindUserByNameAsync(string name, CancellationToken token)
		{	
			if(string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException(nameof(name));
			}

			await Task.Yield();

			lock(_lockObj)
			{
				return _users.FirstOrDefault(itr => itr.Name == name);
			}
		}

		Task INugetUserRepositoryWriter.UpdateUserAsync(string apiKey, string userName, CancellationToken token)
		{
			throw new NotImplementedException();
		}
	}
}

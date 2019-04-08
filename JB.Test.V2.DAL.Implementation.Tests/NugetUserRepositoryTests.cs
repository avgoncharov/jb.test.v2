using AutoFixture.Xunit2;
using JB.Test.V2.DAL.Implementation.DB;
using JB.Test.V2.DAL.Interfaces;
using JB.Test.V2.DAL.Interfaces.Exceptions;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Xunit;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	public sealed class NugetUserRepositoryTests : RepositoryTestsBase
	{
		private readonly IUnityContainer _container;

		[Theory, AutoData]
		public async Task CreateUserTest(string apiKey, string name)
		{
			var store = Container.Resolve<NugetStore>();
			Assert.False(await store.Users.AnyAsync());

			var repo = Container.Resolve<INugetUserRepositoryWriter>();
			await repo.CreateUserAsync(apiKey, name, CancellationToken.None);


			var user = await store.Users.SingleAsync(itr => itr.ApiKey == apiKey);
			Assert.Equal(name, user.Name);
		}

		[Theory, AutoData]
		public async Task CreateUserTwiceTest(string apiKey, string name)
		{
			var store = Container.Resolve<NugetStore>();
			Assert.False(await store.Users.AnyAsync());

			var repo = Container.Resolve<INugetUserRepositoryWriter>();
			await repo.CreateUserAsync(apiKey, name, CancellationToken.None);
			var exeption = await Assert.ThrowsAsync<UserIsAlreadyExistException>(async () => await repo.CreateUserAsync(apiKey, name, CancellationToken.None));			
		}


		[Theory, AutoData]
		public async Task UpdateUserTest(string apiKey, string name, string newName)
		{
			var store = Container.Resolve<NugetStore>();
			Assert.False(await store.Users.AnyAsync());

			var repo = Container.Resolve<INugetUserRepositoryWriter>();
			await repo.CreateUserAsync(apiKey, name, CancellationToken.None);
			await repo.UpdateUserAsync(apiKey, newName, CancellationToken.None);
			
			var user = await store.Users.SingleAsync(itr => itr.ApiKey == apiKey);
			Assert.Equal(newName, user.Name);
		}


		[Theory, AutoData]
		public async Task UpdateUserThatNotExistTest(string apiKey, string name)
		{							
			var repo = Container.Resolve<INugetUserRepositoryWriter>();											      			
			var exception = await Assert.ThrowsAsync<UserNotFoundException>(async()=> await repo.UpdateUserAsync(apiKey, name, CancellationToken.None));			
		}


		[Theory, AutoData]
		public async Task DeleteUserTest(string apiKey, string name)
		{
			var store = Container.Resolve<NugetStore>();
			Assert.False(await store.Users.AnyAsync());

			var repo = Container.Resolve<INugetUserRepositoryWriter>();
			await repo.CreateUserAsync(apiKey, name, CancellationToken.None);

			Assert.Single(store.Users);

			await repo.DeleteUserAsync(apiKey, CancellationToken.None);
			Assert.False(await store.Users.AnyAsync());
		}


		[Theory, AutoData]
		public async Task FindUserByApiKeyTest(string apiKey, string name)
		{
			var store = Container.Resolve<NugetStore>();
			Assert.False(await store.Users.AnyAsync());

			var repoWriter = Container.Resolve<INugetUserRepositoryWriter>();
			await repoWriter.CreateUserAsync(apiKey, name, CancellationToken.None);

			var repoReader = Container.Resolve<INugetUserRepositoryReader>();
			var user = await repoReader.FindUserByApiKeyAsync(apiKey, CancellationToken.None);

			Assert.NotNull(user);
			Assert.Equal(name, user.Name);
			Assert.Equal(apiKey, user.ApiKey);

			var notFoundUserName = await repoReader.FindUserByApiKeyAsync(apiKey+name, CancellationToken.None);
			Assert.Null(notFoundUserName);
		}


		[Theory, AutoData]
		public async Task FindUserNameTest(string apiKey, string name)
		{
			var store = Container.Resolve<NugetStore>();
			Assert.False(await store.Users.AnyAsync());

			var repoWriter = Container.Resolve<INugetUserRepositoryWriter>();
			await repoWriter.CreateUserAsync(apiKey, name, CancellationToken.None);

			var repoReader = Container.Resolve<INugetUserRepositoryReader>();
			var user = await repoReader.FindUserByNameAsync(name, CancellationToken.None);

			Assert.NotNull(user);
			Assert.Equal(name, user.Name);
			Assert.Equal(apiKey, user.ApiKey);

			var notFoundUserName = await repoReader.FindUserByNameAsync(apiKey + name, CancellationToken.None);
			Assert.Null(notFoundUserName);
		}


		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();
			Container.RegisterType<INugetUserRepositoryWriter, NugetUserRepositoryWriter>();
			Container.RegisterType<INugetUserRepositoryReader, NugetUserRepositoryReader>();
		}


		protected override void Dispose(bool disposing)
		{
			var store = new NugetStore();
			store.Database.ExecuteSqlCommand("truncate table dbo.NugetUsers");

			base.Dispose(disposing);
		}
	}
}

using JB.Test.V2.DAL.Interfaces;

namespace JB.Test.V2.DAL.Implementation
{
	/// <summary>
	/// The class of nuget user, implements INugetUser.
	/// </summary>
	internal sealed class NugetUser : INugetUser
	{	
		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="apiKey">User's api key.</param>
		/// <param name="name">User's name.</param>
		internal NugetUser(string apiKey, string name)
		{
			ApiKey = apiKey;
			Name = name;
		}

		/// <inheritdoc/>
		public string ApiKey { get; }


		/// <inheritdoc/>
		public string Name { get; }
	}
}

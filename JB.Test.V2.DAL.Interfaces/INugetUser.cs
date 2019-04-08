namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// The interface of nuget user.
	/// </summary>
	public interface INugetUser
	{			     
		/// <summary>
		/// Gets api key of user.
		/// </summary>
		string ApiKey { get; }

		/// <summary>
		/// Gets user's name.
		/// </summary>
		string Name { get; }
	}
}

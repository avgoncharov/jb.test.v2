namespace JB.Test.V2.DAL.Interfaces
{
	/// <summary>
	/// Class of a filter for package searching.
	/// </summary>
	public sealed class Filter
	{
		/// <summary>
		/// Gets / sets id template.
		/// </summary>
		public string Id { get; set; }


		/// <summary>
		/// Gets / sets version template.
		/// </summary>
		public string Version { get; set; }


		/// <summary>
		/// Gets / sets description template.
		/// </summary>
		public string Description { get; set; }

		//
		public int PageNumber { get; set; }
		public int RowsPerPage { get; set; }

		public bool IsEmpty()
		{
			return 
				string.IsNullOrWhiteSpace(Id)
				&& string.IsNullOrWhiteSpace(Version)
				&& string.IsNullOrWhiteSpace(Description);
		}
	}
}

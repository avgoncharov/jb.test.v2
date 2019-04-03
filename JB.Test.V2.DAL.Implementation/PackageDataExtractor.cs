using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace JB.Test.V2.DAL.Implementation
{
	/// <summary>
	/// Class of package's data extractor.
	/// </summary>
	internal static class PackageDataExtractor
	{
		/// <summary>
		/// Extracts metadata from file.
		/// </summary>
		/// <param name="path">File's path.</param>
		/// <param name="token">Cancellation token.</param>
		/// <returns></returns>
		internal static async Task<PackageMetadata> ExtractAync(string path, CancellationToken token)
		{
			if(string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("Path can't be empty.");
			}

			if(File.Exists(path) != true)
			{
				throw new FileNotFoundException($"Can't find file '{path}'.");
			}

			var rawMetadata = await ReadRawMetadata(path, token);

			if(string.IsNullOrWhiteSpace(rawMetadata))
			{
				throw new InvalidDataException("Bad data format.");
			}

			var result = new PackageMetadata { Metadata = rawMetadata };

			//Warn: bad idea.
			rawMetadata = Regex.Replace(rawMetadata, @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""", "");

			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(rawMetadata);

			var idElem = xmlDoc.SelectSingleNode("//metadata/id");
			if(idElem == null || string.IsNullOrWhiteSpace(idElem.InnerText))
				throw new InvalidDataException("Id can't be empty.");

			result.Id = idElem.InnerText;

			var versionElem = xmlDoc.SelectSingleNode("//metadata/version");
			if(versionElem == null || string.IsNullOrWhiteSpace(versionElem.InnerText))
				throw new InvalidDataException("version can't be empty.");

			result.Version = versionElem.InnerText;

			var descriptionElem = xmlDoc.SelectSingleNode("//metadata/description");
			if(descriptionElem != null)
			{
				result.Description = descriptionElem.InnerText;
			}

			return result;
		}

		private static async Task<string> ReadRawMetadata(string path, CancellationToken token)
		{
			using(var data = File.Open(path, FileMode.Open))
			using(var archive = new ZipArchive(data, ZipArchiveMode.Read))
			{
				foreach(var entry in archive.Entries)
				{
					if(entry.FullName.EndsWith(".nuspec"))
					{
						using(var stream = new StreamReader(entry.Open()))
						{
							return await stream.ReadToEndAsync();
						}
					}
				}

				throw new InvalidDataException("Bad data format.");
			}
		}
	}
}

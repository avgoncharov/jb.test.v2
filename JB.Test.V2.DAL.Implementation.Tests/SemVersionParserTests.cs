using System;
using System.IO;
using AutoFixture.Xunit2;
using Xunit;

namespace JB.Test.V2.DAL.Implementation.Tests
{
	/// <summary>
	/// Test suite for a SemVersionParser.
	/// </summary>
	public sealed class SemVersionParserTests
	{
		/// <summary>
		/// Tests a normal execution.
		/// </summary>
		/// <param name="major">Major part of sem version.</param>
		/// <param name="minor">Minor part of sem version.</param>
		/// <param name="patch">Patch part of sem version.</param>
		/// <param name="suffix">Suffix of sem version.</param>
		[Theory, AutoData]
		public void NormalStringTest(int major, int minor, int patch, string suffix)
		{									    
			var version = SemVersionParser.Parse($"{major}.{minor}.{patch}-{suffix}");

			Assert.Equal(major, version.Major);
			Assert.Equal(minor, version.Minor);
			Assert.Equal(patch, version.Patch);
			Assert.Equal(suffix, version.VersionSuffix);

			var version2 = SemVersionParser.Parse($"{major}.{minor}.{patch}");

			Assert.Equal(major, version2.Major);
			Assert.Equal(minor, version2.Minor);
			Assert.Equal(patch, version2.Patch);
			Assert.Empty(version2.VersionSuffix);
		}


		/// <summary>
		/// Tests an execution with null and empty string as parameter.
		/// </summary>
		[Fact]
		public void EmptyAndNullStringTest()
		{
			Assert.Throws<ArgumentException>(() => SemVersionParser.Parse(null));
			Assert.Throws<ArgumentException>(() => SemVersionParser.Parse(string.Empty));
		}


		/// <summary>
		/// Tests an execution with bad input format.
		/// </summary>
		[Fact]
		public void InvalidDataInStringTest()
		{
			Assert.Throws<InvalidDataException>(() => SemVersionParser.Parse("abc.cb.dd"));
			Assert.Throws<InvalidDataException>(() => SemVersionParser.Parse("123-dd"));
		}
	}
}

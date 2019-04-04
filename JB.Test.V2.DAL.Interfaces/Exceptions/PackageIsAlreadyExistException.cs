using System;
using System.Runtime.Serialization;

namespace JB.Test.V2.DAL.Interfaces.Exceptions
{
	[Serializable]
	public class PackageIsAlreadyExistException: Exception
	{
		public PackageIsAlreadyExistException() : base()
		{
		}

		public PackageIsAlreadyExistException(string message) : base(message)
		{
		}

		public PackageIsAlreadyExistException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public PackageIsAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace JB.Test.V2.DAL.Interfaces.Exceptions
{
	[Serializable]
	public sealed class UserNotFoundException : Exception
	{
		public UserNotFoundException() : base()
		{
		}

		public UserNotFoundException(string message) : base(message)
		{
		}

		public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

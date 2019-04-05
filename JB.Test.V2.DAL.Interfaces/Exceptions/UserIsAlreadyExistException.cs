using System;
using System.Runtime.Serialization;

namespace JB.Test.V2.DAL.Interfaces.Exceptions
{
	[Serializable]
	public sealed class UserIsAlreadyExistException	 :Exception
	{
		public UserIsAlreadyExistException() : base()
		{
		}

		public UserIsAlreadyExistException(string message) : base(message)
		{
		}

		public UserIsAlreadyExistException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public UserIsAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

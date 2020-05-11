using System;
using System.Runtime.Serialization;

namespace DanielBogdan.Passwordless.Identity.Core
{
    public class PasswordlessException : Exception
    {
        public PasswordlessException()
        {
        }

        public PasswordlessException(string message) : base(message)
        {
        }

        public PasswordlessException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PasswordlessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

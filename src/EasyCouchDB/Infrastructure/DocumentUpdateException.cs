using System;
using System.Runtime.Serialization;

namespace EasyCouchDB.Infrastructure
{
    public class DocumentUpdateException : Exception
    {
        public DocumentUpdateException()
        {
        }

        public DocumentUpdateException(string message) : base(message)
        {
        }

        public DocumentUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
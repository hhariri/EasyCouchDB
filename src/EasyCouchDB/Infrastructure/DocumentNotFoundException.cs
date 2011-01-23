using System;
using System.Runtime.Serialization;

namespace EasyCouchDB.Infrastructure
{
    public class DocumentNotFoundException : Exception
    {
        public DocumentNotFoundException()
        {
        }

        public DocumentNotFoundException(string message) : base(message)
        {
        }

        public DocumentNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
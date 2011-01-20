using System;
using System.Runtime.Serialization;

namespace EasyCouchDB
{
    public class DocumentConflictException : Exception
    {
        public DocumentConflictException()
        {
        }

        public DocumentConflictException(string message) : base(message)
        {
        }

        public DocumentConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
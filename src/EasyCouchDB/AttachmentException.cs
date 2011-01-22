using System;
using System.Runtime.Serialization;

namespace EasyCouchDB
{
    public class AttachmentException : Exception
    {
        public AttachmentException()
        {
        }

        public AttachmentException(string message) : base(message)
        {
        }

        public AttachmentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AttachmentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
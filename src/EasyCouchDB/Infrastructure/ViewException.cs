﻿using System;
using System.Runtime.Serialization;

namespace EasyCouchDB.Infrastructure
{
    public class ViewException : Exception
    {
        public ViewException()
        {
        }

        public ViewException(string message) : base(message)
        {
        }

        public ViewException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ViewException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
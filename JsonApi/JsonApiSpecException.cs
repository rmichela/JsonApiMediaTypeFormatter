using System;

namespace JsonApi
{
    public class JsonApiSpecException : Exception
    {
        public JsonApiSpecException(string message) : base(message)
        {
        }

        public JsonApiSpecException(string format, params object[] args) : this(string.Format(format, args))
        {           
        }

        public JsonApiSpecException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

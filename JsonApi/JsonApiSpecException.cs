using System;

namespace JsonApi
{
    public class JsonApiSpecException : Exception
    {
        public JsonApiSpecException()
        {
        }

        public JsonApiSpecException(string message) : base(message)
        {
        }

        public JsonApiSpecException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

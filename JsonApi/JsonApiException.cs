using System;

namespace JsonApi
{
    public class JsonApiException : Exception
    {
        public JsonApiException()
        {
        }

        public JsonApiException(string message) : base(message)
        {
        }

        public JsonApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

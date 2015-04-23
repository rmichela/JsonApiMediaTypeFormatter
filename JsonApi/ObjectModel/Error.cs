namespace JsonApi.ObjectModel
{
    public class Error
    {
        /// <summary>
        /// A unique identifier for this particular occurrence of the problem.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A URI that MAY yield further details about this particular occurrence of the problem.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// The HTTP status code applicable to this problem, expressed as a string value.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// An application-specific error code, expressed as a string value.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// A short, human-readable summary of the problem. It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A human-readable explanation specific to this occurrence of the problem.
        /// </summary>
        public string Detail { get; set; }
    }
}

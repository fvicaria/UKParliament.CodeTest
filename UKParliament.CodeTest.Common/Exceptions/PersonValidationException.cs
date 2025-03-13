
namespace UKParliament.CodeTest.Common.Exceptions
{
    /// <summary>
    /// Thrown when validation checks fail for a <see cref="Person"/> operation.
    /// </summary>
    public class PersonValidationException : Exception
    {
        public PersonValidationException()
        {
        }

        public PersonValidationException(string message)
            : base(message)
        {
        }

        public PersonValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}


namespace UKParliament.CodeTest.Common.Exceptions
{
    /// <summary>
    /// Thrown when an operation requires a <see cref="Person"/> that does not exist.
    /// </summary>
    public class PersonNotFoundException : Exception
    {
        public PersonNotFoundException()
        {
        }

        public PersonNotFoundException(string message)
            : base(message)
        {
        }

        public PersonNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

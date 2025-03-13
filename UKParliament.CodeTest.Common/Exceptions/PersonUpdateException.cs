
namespace UKParliament.CodeTest.Common.Exceptions
{
    public class PersonUpdateException : Exception
    {
        public PersonUpdateException(string message)
            : base(message) { }

        public PersonUpdateException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}

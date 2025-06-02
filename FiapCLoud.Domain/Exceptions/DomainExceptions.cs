using System.Net;

namespace FiapCLoud.Domain.Exceptions;

public class DomainException : Exception, IStatusCodeException
{
    public int StatusCode { get; }

    public DomainException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = (int)statusCode;
    }

    public DomainException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException)
    {
        StatusCode = (int)statusCode;
    }
}

using System.Net;

namespace FiapCLoud.Domain.Exceptions;

public class Validations
{

    public static void ValidateIfNull(object obj, string message)
    {
        if (obj is null)
            throw new DomainException(message, HttpStatusCode.BadRequest);
    }

    public static void ValidateIfNullOrEmpty(string text, string message)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new DomainException(message, HttpStatusCode.BadRequest);
    }

    public static void ValidateIfNegative(decimal number, string message)
    {
        if (number < 0)
            throw new DomainException(message, HttpStatusCode.BadRequest);
    }

    public static void ValidateIfLessOrEqualZero(int number, string message)
    {
        if (number <= 0)
            throw new DomainException(message, HttpStatusCode.BadRequest);
    }

    public static void ValidateIfTrue(bool condition, string message)
    {
        if (condition)
            throw new DomainException(message, HttpStatusCode.BadRequest);
    }

    public static void ValidateIfFalse(bool condition, string message)
    {
        if (!condition)
            throw new DomainException(message, HttpStatusCode.NotFound);
    }

    public static void ValidateIfLessOrEqualZero(decimal number, string message)
    {
        if (number <= 0)
            throw new DomainException(message, HttpStatusCode.BadRequest);
    }
}

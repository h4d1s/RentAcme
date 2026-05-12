namespace Payment.Application.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException() : base("Unauthorized") { }

    public AuthenticationException(string message) : base(message) { }

    public AuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    { }
}

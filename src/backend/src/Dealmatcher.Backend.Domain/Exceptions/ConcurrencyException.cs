namespace Dealmatcher.Backend.Domain.Exceptions;

public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException()
        : base("Resource was modified by another user.") { }

    public ConcurrencyException(string message)
        : base(message) { }

    public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException) { }
}

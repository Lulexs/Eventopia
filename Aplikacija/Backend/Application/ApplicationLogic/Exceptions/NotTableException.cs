namespace Backend.ApplicationLogic.Exceptions;

public class NotTableException : Exception
{
    public NotTableException(string message) : base(message) { }
}
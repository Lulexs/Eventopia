namespace Backend.ApplicationLogic.Exceptions;

public class EventOwnerException : Exception
{
    public EventOwnerException(string message) : base(message) { }
}
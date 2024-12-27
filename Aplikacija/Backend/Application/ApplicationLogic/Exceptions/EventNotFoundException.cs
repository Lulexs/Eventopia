namespace Backend.ApplicationLogic.Exceptions;

public class EventNotFoundException : Exception
{
    public EventNotFoundException(string message) : base(message) { }
}
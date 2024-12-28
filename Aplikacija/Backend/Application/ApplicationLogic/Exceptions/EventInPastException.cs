namespace Backend.ApplicationLogic.Exceptions;

public class EventInPastException : Exception
{
    public EventInPastException(string message) : base(message)
    {

    }
}
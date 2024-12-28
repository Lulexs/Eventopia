namespace Backend.ApplicationLogic.Exceptions;

public class InvalidDateTimeFormat : Exception
{
    public InvalidDateTimeFormat(string message) : base(message) { }
}
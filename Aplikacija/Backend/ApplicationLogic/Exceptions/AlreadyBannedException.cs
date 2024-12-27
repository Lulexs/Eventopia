namespace Backend.ApplicationLogic.Exceptions;

public class AlreadyBannedException : Exception
{
    public AlreadyBannedException(string message) : base(message) { }
}
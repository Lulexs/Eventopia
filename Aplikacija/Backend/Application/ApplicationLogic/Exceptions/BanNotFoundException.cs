namespace Backend.ApplicationLogic.Exceptions;

public class BanNotFoundException : Exception
{
    public BanNotFoundException(string message) : base(message) { }
}
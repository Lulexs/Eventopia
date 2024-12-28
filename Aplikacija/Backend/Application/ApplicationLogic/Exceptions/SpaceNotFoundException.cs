namespace Backend.ApplicationLogic.Exceptions;

public class SpaceNotFoundException : Exception
{
    public SpaceNotFoundException(string message) : base(message) { }
}
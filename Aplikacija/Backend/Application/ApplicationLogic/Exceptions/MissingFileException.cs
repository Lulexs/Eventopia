namespace Backend.ApplicationLogic.Exceptions;

public class MissingFileException : Exception
{
    public MissingFileException(string message) : base(message) { }
}
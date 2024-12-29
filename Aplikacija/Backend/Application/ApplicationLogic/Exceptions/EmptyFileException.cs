namespace Backend.ApplicationLogic.Exceptions;

public class EmptyFileException : Exception
{
    public EmptyFileException(string message) : base(message) { }
}
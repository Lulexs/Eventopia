namespace Backend.ApplicationLogic.Exceptions;

public class CommentNotFoundException : Exception
{
    public CommentNotFoundException(string message) : base(message) { }
}
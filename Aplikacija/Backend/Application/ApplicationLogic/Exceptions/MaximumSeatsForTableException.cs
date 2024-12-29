namespace Backend.ApplicationLogic.Exceptions;

public class MaximumSeatsForTableException : Exception
{
    public MaximumSeatsForTableException(string message) : base(message) { }
}
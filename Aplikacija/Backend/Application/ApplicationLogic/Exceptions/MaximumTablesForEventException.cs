namespace Backend.ApplicationLogic.Exceptions;

public class MaximumTablesForEventException : Exception
{
    public MaximumTablesForEventException(string message) : base(message)
    {

    }
}
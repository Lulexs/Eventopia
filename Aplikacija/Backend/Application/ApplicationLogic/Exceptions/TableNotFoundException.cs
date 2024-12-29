namespace Backend.ApplicationLogic.Exceptions;

public class TableNotFoundException : Exception
{
    public TableNotFoundException(string message) : base(message) { }
}
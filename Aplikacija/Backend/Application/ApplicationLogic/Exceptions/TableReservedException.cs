namespace Backend.ApplicationLogic.Exceptions;

public class TableReservedException : Exception
{
    public TableReservedException(string message) : base(message) { }
}
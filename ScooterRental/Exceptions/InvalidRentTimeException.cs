namespace ScooterRental.Exceptions;

public class InvalidRentTimeException : Exception
{
    public InvalidRentTimeException(string message) : base(message)
    {
    }
}
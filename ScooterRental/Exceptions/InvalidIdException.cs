namespace ScooterRental.Exceptions;

public class InvalidIdException : Exception
{
    public InvalidIdException() : base("Provided ID is not valid")
    {
        
    }
}
namespace ScooterRental.Exceptions;

public class ScooterNotFoundException : Exception
{
    public ScooterNotFoundException() : base("This scooter is not found")
    {
        
    }
}
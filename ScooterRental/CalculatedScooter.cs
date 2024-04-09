namespace ScooterRental;

public class CalculatedScooter
{
    public string ScooterId { get; }
    public DateTime RentEnd { get; }
    public decimal RentPrice { get; }
    public bool IsFinished { get; }

    public CalculatedScooter(string scooterId, DateTime rentEnd, decimal rentPrice, bool isFinished)
    {
        ScooterId = scooterId;
        RentEnd = rentEnd;
        RentPrice = rentPrice;
        IsFinished = isFinished;
    }
}
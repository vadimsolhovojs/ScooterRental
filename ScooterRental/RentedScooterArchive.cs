using ScooterRental.Exceptions;

namespace ScooterRental;

public class RentedScooterArchive : IRentedScooterArchive
{
    private readonly List<RentedScooter> _rentedScooterArchive;

    public RentedScooterArchive(List<RentedScooter> rentedScooterArchive)
    {
        _rentedScooterArchive = rentedScooterArchive;
    }
    public void AddRentedScooter(RentedScooter scooter)
    {
        var sameScooter = _rentedScooterArchive.LastOrDefault(archiveScooter => 
            archiveScooter.ScooterId == scooter.ScooterId);
        
        if (sameScooter != null)
        {
            if (sameScooter.RentStart == scooter.RentStart)
            {
                throw new DuplicateScooterException();
            }
            
            if (sameScooter.RentEnd == null || sameScooter.RentEnd > scooter.RentStart)
            {
                throw new InvalidRentTimeException("Cannot rent already rented scooter.");
            }
        }
        
        if (scooter.PricePerMinute <= 0 )
        {
            throw new InvalidPriceException();
        }
        
        _rentedScooterArchive.Add(scooter);
    }

    public RentedScooter EndRental(string scooterId, DateTime rentEnd)
    {
        var rentedScooter = _rentedScooterArchive.SingleOrDefault(scooter => scooter.ScooterId == scooterId);
        
        if (rentedScooter == null)
        {
            throw new InvalidScooterIdException();
        }
        
        if (rentedScooter.RentEnd != null)
        {
            throw new ScooterIsNotRentedException();
        }
        
        rentedScooter.RentEnd = rentEnd;

        return rentedScooter;
    }
}
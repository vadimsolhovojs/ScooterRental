namespace ScooterRental;

public interface IRentedScooterArchive
{
    void AddRentedScooter(RentedScooter scooter);

    RentedScooter EndRental(string scooterId, DateTime rentEnd);
}
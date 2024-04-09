namespace ScooterRental;

public interface IRentalCalculatorService
{
    decimal CalculateRent(RentedScooter rentalRecord);
    decimal CalculateIncome(int? year, bool includeNotCompletedRentals);
}
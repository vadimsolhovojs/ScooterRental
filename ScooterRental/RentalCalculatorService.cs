using System;
using ScooterRental.Exceptions;

namespace ScooterRental;

public class RentalCalculatorService : IRentalCalculatorService
{
    private readonly List<CalculatedScooter> _calculatedScooters;
    private const decimal MaxDailyPrice = 20m;

    public RentalCalculatorService(List<CalculatedScooter> calculatedScooters)
    {
        _calculatedScooters = calculatedScooters;
    }

    private TimeSpan GetRentInterval(RentedScooter rentalRecord)
    {
        TimeSpan rentInterval;
        if (rentalRecord.RentEnd == null)
        {
            rentInterval = DateTime.Now - rentalRecord.RentStart;
            if (rentInterval.Minutes <= 0)
            {
                throw new InvalidRentTimeException("Invalid rent interval");
            }

            return rentInterval;
        }

        rentInterval = rentalRecord.RentEnd.Value - rentalRecord.RentStart;
        if (rentInterval.Minutes <= 0)
        {
            throw new InvalidRentTimeException("Invalid rent interval");
        }

        return rentInterval;
    }
    private bool ApplyMaxDailyPrice(decimal pricePerMinute)
    {
        return pricePerMinute * 60 * 24 >= MaxDailyPrice;
    }

    private void AddCalculatedScooter(RentedScooter rentalRecord, decimal rentPrice)
    {
        if (rentalRecord.RentEnd == null)
        {
            _calculatedScooters.Add(new CalculatedScooter(rentalRecord.ScooterId, DateTime.Now, rentPrice, false));
        }
        if (rentalRecord.RentEnd != null)
        {
            var notCompleted = _calculatedScooters.SingleOrDefault(s => s.ScooterId == rentalRecord.ScooterId && !s.IsFinished);

            if (notCompleted != null)
            {
                _calculatedScooters.Remove(notCompleted);
            }
            
            _calculatedScooters.Add(new CalculatedScooter(rentalRecord.ScooterId, rentalRecord.RentEnd.Value, rentPrice, true));
        }
    }
    
    public decimal CalculateRent(RentedScooter rentalRecord)
    {
        TimeSpan rentInterval = GetRentInterval(rentalRecord);

        var fullDays = rentInterval.Days;
        var minutesLeft = rentInterval.Hours * 60 + rentInterval.Minutes;
        decimal result = 0;
        
        if (ApplyMaxDailyPrice(rentalRecord.PricePerMinute))
        {
            result += MaxDailyPrice * fullDays;
        }
        else
        {
            result = rentalRecord.PricePerMinute * 24 * 60 * fullDays;
        }

        result += rentalRecord.PricePerMinute * minutesLeft;

        AddCalculatedScooter(rentalRecord, result);

        return result;
    }

    public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
    {
        decimal result = 0;
        
        foreach (var rental in _calculatedScooters)
        {
            if ((year == null || rental.RentEnd.Year == year) &&
                (includeNotCompletedRentals || rental.IsFinished))
            {
                result += rental.RentPrice;
            }
        }

        return result;
    }
}
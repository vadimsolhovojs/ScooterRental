using FluentAssertions;
using Moq;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests;

[TestClass]
public class RentalCalculatorServiceTests
{
    private IRentalCalculatorService _rentalCalculatorService;
    private List<CalculatedScooter> _calculatedScooters;
    
    [TestInitialize]
    public void Setup()
    {
        _calculatedScooters = new();
        _rentalCalculatorService = new RentalCalculatorService(_calculatedScooters);
    }
    
    [TestMethod]
    public void CalculateRent_Valid_RentedScooter_With_RentEnd_Provided_Calculated_Cost_Expected()
    {
        RentedScooter rentalRecord = new RentedScooter("1", new DateTime(2024, 2, 2, 00, 00, 00), 0.1m)
            { RentEnd = new DateTime(2024, 2, 7, 02, 10, 00) };
        
        var result = _rentalCalculatorService.CalculateRent(rentalRecord);
  
        result.Should().Be(113m);
        _calculatedScooters.Count.Should().Be(1);
        _calculatedScooters[0].IsFinished.Should().BeTrue();
    }
    
    [TestMethod]
    public void CalculateRent_Valid_RentedScooter_Without_RendEnd_Provided_Calculated_Cost_Expected()
    {
        var now = DateTime.Now;
        RentedScooter rentalRecord = new RentedScooter("1", now.AddMinutes(-20), 0.01m);
        TimeSpan rentInterval = DateTime.Now - rentalRecord.RentStart;
        var cost = (decimal)Math.Floor(rentInterval.TotalMinutes) * rentalRecord.PricePerMinute;

        var result = _rentalCalculatorService.CalculateRent(rentalRecord);
    
        result.Should().Be(cost);
        _calculatedScooters.Count.Should().Be(1);
        _calculatedScooters[0].IsFinished.Should().BeFalse();
    }
    
    [TestMethod]
    public void CalculateRent_invalid_RentEnd_Value_Provided_InvalidRentTimeException_Expected()
    {
        RentedScooter rentalRecord = new RentedScooter("1", new DateTime(2024, 2, 2, 00, 00, 00), 0.1m)
            { RentEnd = new DateTime(2024, 2, 1, 00, 00, 00) };
       
        Action action = () => _rentalCalculatorService.CalculateRent(rentalRecord);
    
        action.Should().Throw<InvalidRentTimeException>();
    }
    
    [TestMethod]
    public void CalculateRent_Invalid_RentStart_Provided_InvalidRentTimeException_Expected()
    {
        var now = DateTime.Now;
        RentedScooter rentalRecord = new RentedScooter("1", now.AddMinutes(20), 0.01m);
        TimeSpan rentInterval = DateTime.Now - rentalRecord.RentStart;
        var cost = (decimal)Math.Floor(rentInterval.TotalMinutes) * rentalRecord.PricePerMinute;
        
        Action action = () => _rentalCalculatorService.CalculateRent(rentalRecord);

        action.Should().Throw<InvalidRentTimeException>();
    }
    
    [TestMethod]
    public void CalculateRent_Valid_RentedScooter_With_RendEnd_Provided_Unfinished_Record_Replaced_Expected()
    {
        var now = DateTime.Now;
        RentedScooter rentalRecordUnfinished = new RentedScooter("1", new DateTime(2024, 2, 2, 00, 00, 00), 0.1m);
        RentedScooter rentalRecordFinished = new RentedScooter("1", new DateTime(2024, 2, 2, 00, 00, 00), 0.1m)
            { RentEnd = new DateTime(2024, 2, 7, 02, 10, 00) };
 
        _rentalCalculatorService.CalculateRent(rentalRecordUnfinished);
        _rentalCalculatorService.CalculateRent(rentalRecordFinished);
        
        _calculatedScooters.Count.Should().Be(1);
        _calculatedScooters[0].IsFinished.Should().BeTrue();
    }

    [TestMethod]
    public void CalculateIncome_Year_And_includeNotCompletedRentals_True_Provided_Income_Value_Expected()
    {
        _calculatedScooters.Add(new CalculatedScooter("1", new DateTime(2023,2,1), 4, true));
        _calculatedScooters.Add(new CalculatedScooter("2", new DateTime(2022,1,1), 5, true));
        _calculatedScooters.Add(new CalculatedScooter("1", new DateTime(2023,1,1), 6, false));
        
        var result = _rentalCalculatorService.CalculateIncome(null, true);
        
        _calculatedScooters.Count.Should().Be(3);
        result.Should().Be(15);
    }
}
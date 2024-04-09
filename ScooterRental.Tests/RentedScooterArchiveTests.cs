using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests;

[TestClass]
public class RentedScooterArchiveTests
{
    private IRentedScooterArchive _rentedScooterArchive;
    private List<RentedScooter> _rentedScooters;
    
    [TestInitialize]
    public void Setup()
    { 
        _rentedScooters = new List<RentedScooter>();
        _rentedScooterArchive = new RentedScooterArchive(_rentedScooters);
    }

    [TestMethod]
    public void AddRentedScooter_Valid_RentedScooter_Provided_RentedScooter_Added_To_List()
    {
        var rentedScooter = new RentedScooter("1", DateTime.Now, 0.1m);
        
        _rentedScooterArchive.AddRentedScooter(rentedScooter);

        _rentedScooters.Count.Should().Be(1);
    }
    
    [TestMethod]
    public void AddRentedScooter_Already_RentedScooter_RentEnd_IsNull_Provided_InvalidRentTimeException_Expected()
    {
        var archiveScooter = new RentedScooter(
            "1", 
            new DateTime(2024, 2,5, 00,00,00), 
            0.1m);
        _rentedScooters.Add(archiveScooter);
        var rentedScooter = new RentedScooter(
            "1", 
            DateTime.Now, 
            0.1m);

        Action action = () => _rentedScooterArchive.AddRentedScooter(rentedScooter);

        action.Should().Throw<InvalidRentTimeException>();
    }
    
    [TestMethod]
    public void AddRentedScooter_Already_RentedScooter_RentEnd_HasValue_Provided_InvalidRentTimeException_Expected()
    {
        var archiveScooter = new RentedScooter(
            "1", 
            new DateTime(2024, 2,5, 00,00,00), 
            0.1m);
        archiveScooter.RentEnd = new DateTime(2024, 2, 10, 00, 00, 00);
        
        _rentedScooters.Add(archiveScooter);
        
        var rentedScooter = new RentedScooter(
            "1", 
            new DateTime(2024, 2,7, 00,00,00), 
            0.1m);

        Action action = () => _rentedScooterArchive.AddRentedScooter(rentedScooter);

        action.Should().Throw<InvalidRentTimeException>();
    }
    
    [TestMethod]
    public void AddRentedScooter_Duplicate_RentedScooter_Provided_DuplicateScooterException_Expected()
    {
        var now = DateTime.Now;
        var archiveScooter = new RentedScooter(
            "1", 
            now, 
            0.1m);
        _rentedScooters.Add(archiveScooter);
        
        Action action = () => _rentedScooterArchive.AddRentedScooter(archiveScooter);

        action.Should().Throw<DuplicateScooterException>();
    }
    
    [TestMethod]
    public void AddRentedScooter_Invalid_RentedScooter_PricePerMinute_Provided_RentedScooter_Added_To_List()
    {
        var now = DateTime.Now;
        var rentedScooter = new RentedScooter("1", now, -0.1m);
        
        Action action = () => _rentedScooterArchive.AddRentedScooter(rentedScooter);

        action.Should().Throw<InvalidPriceException>();
    }

    [TestMethod]
    public void EndRental_Valid_ScooterId_And_RentEnd_Provided_Valid_RentedScooter_Returned()
    {
        var now = DateTime.Now;
        var rentedScooter = new RentedScooter(
            "1", 
            new DateTime(2024, 2,7, 00,00,00), 
            0.1m);
        _rentedScooters.Add(rentedScooter);

        var result = _rentedScooterArchive.EndRental("1", now);

        result.Should().Be(rentedScooter);
        result.RentEnd.Should().Be(now);
    }
    
    [TestMethod]
    public void EndRental_Invalid_ScooterId_Provided_InvalidScooterIdException_Expected()
    {
        var now = DateTime.Now;
        var rentedScooter = new RentedScooter(
            "1", 
            new DateTime(2024, 2,7, 00,00,00), 
            0.1m);
        _rentedScooters.Add(rentedScooter);

        Action action = () => _rentedScooterArchive.EndRental("2", now);

        action.Should().Throw<InvalidScooterIdException>();
    }
    
    [TestMethod]
    public void EndRental_Invalid_RentEnd_Provided_ScooterIsNotRentedException_Expected()
    {
        var now = DateTime.Now;
        var rentedScooter = new RentedScooter(
            "1", 
            new DateTime(2024, 2,7, 00,00,00), 
            0.1m);
        rentedScooter.RentEnd = new DateTime(2024, 2, 7, 12, 00, 00);
        _rentedScooters.Add(rentedScooter);

        Action action = () => _rentedScooterArchive.EndRental("1", now);
    
        action.Should().Throw<ScooterIsNotRentedException>();
    }
}
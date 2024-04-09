using FluentAssertions;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests;

[TestClass]
public class ScooterServiceTests
{
    private IScooterService _scooterService;
    private List<Scooter> _scooters;
    private const string defaultScooterId = "1";

    [TestInitialize]
    public void Setup()
    {
        _scooters = new List<Scooter>();
        _scooterService = new ScooterService(_scooters);
    }
    
    [TestMethod]
    public void AddScooter_Valid_Data_Provided_ScooderAdded()
    {
        _scooterService.AddScooter(defaultScooterId, 0.1m);

        _scooters.Count.Should().Be(1);
    }
    
    [TestMethod]
    public void AddScooter_Invalid_Price_Provided_InvalidPriceException_Expected()
    {
        Action action = () => _scooterService.AddScooter(defaultScooterId, 0.0m);
        
        action.Should().Throw<InvalidPriceException>();
    }
    
    [TestMethod]
    public void AddScooter_Invalid_Id_Provided_InvalidIdException_Expected()
    {
        Action action = () => _scooterService.AddScooter("", 0.1m);

        action.Should().Throw<InvalidIdException>();
    }
    
    [TestMethod]
    public void AddScooter_Add_Duplicate_Scooter_DuplicateScooterException_Expected()
    {
        _scooters.Add(new Scooter(defaultScooterId, 0.1m));

        Action action = () => _scooterService.AddScooter(defaultScooterId, 0.1m);

        action.Should().Throw<DuplicateScooterException>();
    }
    
    [TestMethod]
    public void RemoveScooter_Existing_Scooter_Provided_Scooter_Removed_Expected()
    {
        _scooters.Add(new Scooter(defaultScooterId, 0.1m));

        _scooterService.RemoveScooter(defaultScooterId);

        _scooters.Should().BeEmpty();
    }
    
    [TestMethod]
    public void RemoveScooter_NonExisting_Scooter_Provided_ScooterNotFoundException_Expected()
    {
        Action action = () => _scooterService.RemoveScooter(defaultScooterId);

        action.Should().Throw<ScooterNotFoundException>();
    }

    [TestMethod]
    public void GetScooters_Scooters_Provided_List_Of_Scooters_Returned()
    {
        _scooters.Add(new Scooter("Scooter1", 0.1m));
        _scooters.Add(new Scooter("Scooter2", 0.1m));

        var result = _scooterService.GetScooters();

        result.Count().Should().Be(2);
        result[0].Id.Should().Be("Scooter1");
        result[1].Id.Should().Be("Scooter2");
    }
    
    [TestMethod]
    public void GetScooters_No_Scooters_Provided_List_Of_Scooters_Returned()
    {
        var result = _scooterService.GetScooters();

        result.Should().BeEmpty();
    }
    
    [TestMethod]
    public void GetScootersById_ScooterId_Provided_Scooter_Returned()
    {
        _scooters.Add(new Scooter(defaultScooterId, 0.1m));
        
        var result = _scooterService.GetScooterById(defaultScooterId);

        result.Id.Should().Be(defaultScooterId);
    }
    
    [TestMethod]
    [DataRow(defaultScooterId)]
    [DataRow(null)]
    public void GetScootersById_No_ScooterId_Provided_InvalidScooterIdException_Expected(
        string scooterId)
    {
        Action action = () =>_scooterService.GetScooterById(scooterId);
  
        action.Should().Throw<InvalidScooterIdException>();
    }
}
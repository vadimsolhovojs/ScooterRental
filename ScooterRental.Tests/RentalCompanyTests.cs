using FluentAssertions;
using Moq;
using Moq.AutoMock;
using ScooterRental.Exceptions;

namespace ScooterRental.Tests;

[TestClass]
public class RentalCompanyTests
{
    private AutoMocker _mocker;
    private RentalCompany _company;
    private Mock<IScooterService> _scooterServiceMock;
    private Mock<IRentedScooterArchive> _rentedScooterArchiveMock;
    private Mock<IRentalCalculatorService> _rentalCalculatorMock;
    private const string _defaultCompanyName = "tests";

    [TestInitialize]
    public void Setup()
    {
        _mocker = new AutoMocker();
        _scooterServiceMock = _mocker.GetMock<IScooterService>();
        _rentedScooterArchiveMock = _mocker.GetMock<IRentedScooterArchive>();
        _rentalCalculatorMock = _mocker.GetMock<IRentalCalculatorService>();
        _company = new RentalCompany(
            _defaultCompanyName, 
            _scooterServiceMock.Object, 
            _rentedScooterArchiveMock.Object,
            _rentalCalculatorMock.Object);
    }
    
    [TestMethod]
    public void StartRent_Rent_ExistingScooter_ScooterIsRented()
    {
        var scooter = new Scooter("1", 0.1m);
        _scooterServiceMock.Setup(s => s.GetScooterById("1")).Returns(scooter);
        
        _company.StartRent("1");

        scooter.IsRented.Should().BeTrue();
    }
    
    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    public void StartRent_Rent_Invalid_ScooterId_Provided_InvalidScooterIdException_Expected(string id)
    {
        Action action = () =>_company.StartRent(id);

        action.Should().Throw<InvalidScooterIdException>();
    }
    
    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    public void EndRent_Rent_Invalid_ScooterId_Provided_InvalidScooterIdException_Expected(string id)
    {
        Action action = () =>_company.EndRent(id);

        action.Should().Throw<InvalidScooterIdException>();
    }
    
    [TestMethod]
    public void EndRent_Rent_ExistingScooter_ScooterIsNotRented_Rent_Calculated()
    {
        var scooter = new Scooter("1", 0.1m){IsRented = true};
        var now = DateTime.Now;
        var rentalRecord = new RentedScooter(scooter.Id, now.AddMinutes(-20), scooter.PricePerMinute){RentEnd = now};
        _scooterServiceMock.Setup(s => s.GetScooterById("1")).Returns(scooter);
        _rentedScooterArchiveMock.Setup(archive => archive.EndRental(scooter.Id, It.IsAny<DateTime>()))
            .Returns(rentalRecord);
        _rentalCalculatorMock.Setup(calculator => calculator.CalculateRent(rentalRecord)).Returns(5);
        
        var result = _company.EndRent("1");

        scooter.IsRented.Should().BeFalse();
        result.Should().Be(5);
    }
    
    [TestMethod]
    [DataRow(0)]
    [DataRow(-2024)]
    public void CalculateIncome_Invalid_Year_Provided_InvalidYearException_Expected(int year)
    {
        Action action = () =>_company.CalculateIncome(year, It.IsAny<bool>());

        action.Should().Throw<InvalidYearException>();
    }
    
    [TestMethod]
    public void CalculateIncome_Rent_ExistingScooter_ScooterIsRented()
    {
        _rentalCalculatorMock.Setup(s => s.CalculateIncome(2023, It.IsAny<bool>())).Returns(100);
        
        var result = _company.CalculateIncome(2023, true);
        
        result.Should().Be(100);
    }
}
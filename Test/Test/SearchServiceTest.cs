using Application.Contracts.DTOs;
using Application.Contracts.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class SearchServiceTests
    {
        private readonly Mock<IBusScheduleRepository> _mockBusScheduleRepo;
        private readonly SearchService _searchService;

        public SearchServiceTests()
        {
            _mockBusScheduleRepo = new Mock<IBusScheduleRepository>();
            _searchService = new SearchService(_mockBusScheduleRepo.Object);
        }

        [Fact]
        public async Task SearchAvailableBusesAsync_WithValidData_ReturnsAvailableBuses()
        {
            // Arrange
            var fromCity = "Dhaka";
            var toCity = "Chittagong";
            var journeyDate = new DateTime(2025, 11, 1);

            var mockSchedules = GetMockBusSchedules();
            _mockBusScheduleRepo
                .Setup(x => x.GetSchedulesAsync(fromCity, toCity, It.IsAny<DateTime>()))
                .ReturnsAsync(mockSchedules);

            // Act
            var result = await _searchService.SearchAvailableBusesAsync(fromCity, toCity, journeyDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, bus => Assert.True(bus.SeatsLeft > 0));
        }

        [Fact]
        public async Task SearchAvailableBusesAsync_ConvertsToUtcTime()
        {
            // Arrange
            var fromCity = "Dhaka";
            var toCity = "Chittagong";
            var journeyDate = new DateTime(2025, 11, 1, 10, 0, 0, DateTimeKind.Local);

            _mockBusScheduleRepo
                .Setup(x => x.GetSchedulesAsync(fromCity, toCity, It.IsAny<DateTime>()))
                .ReturnsAsync([]);

            // Act
            await _searchService.SearchAvailableBusesAsync(fromCity, toCity, journeyDate);

            // Assert
            _mockBusScheduleRepo.Verify(
                x => x.GetSchedulesAsync(
                    fromCity,
                    toCity,
                    It.Is<DateTime>(d => d.Kind == DateTimeKind.Utc)),
                Times.Once);
        }

        [Fact]
        public async Task SearchAvailableBusesAsync_ReturnsCorrectDtoMapping()
        {
            // Arrange
            var fromCity = "Dhaka";
            var toCity = "Chittagong";
            var journeyDate = new DateTime(2025, 11, 1);

            var mockSchedules = GetMockBusSchedules();
            _mockBusScheduleRepo
                .Setup(x => x.GetSchedulesAsync(fromCity, toCity, It.IsAny<DateTime>()))
                .ReturnsAsync(mockSchedules);

            // Act
            var result = await _searchService.SearchAvailableBusesAsync(fromCity, toCity, journeyDate);

            // Assert
            var firstBus = result.First();
            Assert.Equal(mockSchedules[0].Id, firstBus.BusScheduleId);
            Assert.Equal(mockSchedules[0].Bus.Name, firstBus.BusName);
            Assert.Equal(mockSchedules[0].Bus.CompanyName, firstBus.CompanyName);
            Assert.Equal(mockSchedules[0].Route.FromCity, firstBus.FromCity);
            Assert.Equal(mockSchedules[0].Route.ToCity, firstBus.ToCity);
            Assert.Equal(mockSchedules[0].StartTime, firstBus.StartTime);
            Assert.Equal(mockSchedules[0].ArrivalTime, firstBus.ArrivalTime);
            Assert.Equal(mockSchedules[0].Price, firstBus.Price);
        }

        [Fact]
        public async Task SearchAvailableBusesAsync_CalculatesCorrectSeatsLeft()
        {
            // Arrange
            var fromCity = "Dhaka";
            var toCity = "Chittagong";
            var journeyDate = new DateTime(2025, 11, 1);

            var schedule = new BusSchedule
            {
                Id = Guid.NewGuid(),
                Bus = new Bus { Name = "Express 101", CompanyName = "Green Line" },
                Route = new Route { FromCity = fromCity, ToCity = toCity },
                StartTime = "08:00 AM",
                ArrivalTime = "02:00 PM",
                Price = 800,
                Seats =
                [
                    new Seat { SeatNumber = "A1", Status = "Available" },
                    new Seat { SeatNumber = "A2", Status = "Booked" },
                    new Seat { SeatNumber = "A3", Status = "Available" },
                    new Seat { SeatNumber = "A4", Status = "Sold" }
                ]
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetSchedulesAsync(fromCity, toCity, It.IsAny<DateTime>()))
                .ReturnsAsync([schedule]);

            // Act
            var result = await _searchService.SearchAvailableBusesAsync(fromCity, toCity, journeyDate);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result[0].SeatsLeft);
        }

        [Fact]
        public async Task SearchAvailableBusesAsync_WithNoSchedules_ReturnsEmptyList()
        {
            // Arrange
            var fromCity = "Dhaka";
            var toCity = "Sylhet";
            var journeyDate = new DateTime(2025, 11, 1);

            _mockBusScheduleRepo
                .Setup(x => x.GetSchedulesAsync(fromCity, toCity, It.IsAny<DateTime>()))
                .ReturnsAsync([]);

            // Act
            var result = await _searchService.SearchAvailableBusesAsync(fromCity, toCity, journeyDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAvailableBusesAsync_WithAllSeatsBooked_ReturnsZeroSeatsLeft()
        {
            // Arrange
            var fromCity = "Dhaka";
            var toCity = "Chittagong";
            var journeyDate = new DateTime(2025, 11, 1);

            var schedule = new BusSchedule
            {
                Id = Guid.NewGuid(),
                Bus = new Bus { Name = "Express 101", CompanyName = "Green Line" },
                Route = new Route { FromCity = fromCity, ToCity = toCity },
                StartTime = "08:00 AM",
                ArrivalTime = "02:00 PM",
                Price = 800,
                Seats =
                [
                    new Seat { SeatNumber = "A1", Status = "Booked" },
                    new Seat { SeatNumber = "A2", Status = "Sold" }
                ]
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetSchedulesAsync(fromCity, toCity, It.IsAny<DateTime>()))
                .ReturnsAsync([schedule]);

            // Act
            var result = await _searchService.SearchAvailableBusesAsync(fromCity, toCity, journeyDate);

            // Assert
            Assert.Single(result);
            Assert.Equal(0, result[0].SeatsLeft);
        }

        private static List<BusSchedule> GetMockBusSchedules()
        {
            var bus1 = new Bus
            {
                Id = Guid.NewGuid(),
                Name = "Express 101",
                CompanyName = "Green Line",
                TotalSeats = 40
            };

            var bus2 = new Bus
            {
                Id = Guid.NewGuid(),
                Name = "Deluxe 202",
                CompanyName = "Shyamoli",
                TotalSeats = 36
            };

            var route = new Route
            {
                Id = Guid.NewGuid(),
                FromCity = "Dhaka",
                ToCity = "Chittagong"
            };

            var schedule1 = new BusSchedule
            {
                Id = Guid.NewGuid(),
                BusId = bus1.Id,
                Bus = bus1,
                RouteId = route.Id,
                Route = route,
                JourneyDate = new DateTime(2025, 11, 1),
                StartTime = "08:00 AM",
                ArrivalTime = "02:00 PM",
                Price = 800,
                Seats =
                [
                    new Seat { SeatNumber = "A1", Status = "Available" },
                    new Seat { SeatNumber = "A2", Status = "Available" },
                    new Seat { SeatNumber = "A3", Status = "Booked" }
                ]
            };

            var schedule2 = new BusSchedule
            {
                Id = Guid.NewGuid(),
                BusId = bus2.Id,
                Bus = bus2,
                RouteId = route.Id,
                Route = route,
                JourneyDate = new DateTime(2025, 11, 1),
                StartTime = "10:00 AM",
                ArrivalTime = "04:00 PM",
                Price = 1000,
                Seats =
                [
                    new Seat { SeatNumber = "B1", Status = "Available" },
                    new Seat { SeatNumber = "B2", Status = "Available" }
                ]
            };

            return [schedule1, schedule2];
        }
    }
}
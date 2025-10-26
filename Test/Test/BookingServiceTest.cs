using Application.Contracts.DTOs;
using Application.Contracts.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBusScheduleRepository> _mockBusScheduleRepo;
        private readonly Mock<ISeatRepository> _mockSeatRepo;
        private readonly Mock<ITicketRepository> _mockTicketRepo;
        private readonly Mock<IPassengerRepository> _mockPassengerRepo;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBusScheduleRepo = new Mock<IBusScheduleRepository>();
            _mockSeatRepo = new Mock<ISeatRepository>();
            _mockTicketRepo = new Mock<ITicketRepository>();
            _mockPassengerRepo = new Mock<IPassengerRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _bookingService = new BookingService(
                _mockBusScheduleRepo.Object,
                _mockSeatRepo.Object,
                _mockTicketRepo.Object,
                _mockPassengerRepo.Object,
                _mockUnitOfWork.Object);
        }

        #region GetSeatPlanAsync Tests

        [Fact]
        public async Task GetSeatPlanAsync_WithValidScheduleId_ReturnsSeatPlan()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.GetSeatPlanAsync(scheduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Seats.Count);
            Assert.Contains(result.Seats, s => s.SeatNumber == "A1" && s.Status == "Available");
            Assert.Contains(result.Seats, s => s.SeatNumber == "A2" && s.Status == "Booked");
            Assert.Contains(result.Seats, s => s.SeatNumber == "A3" && s.Status == "Available");
        }

        [Fact]
        public async Task GetSeatPlanAsync_MapsSeatsCorrectly()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.GetSeatPlanAsync(scheduleId);

            // Assert
            for (int i = 0; i < mockSchedule.Seats.Count; i++)
            {
                Assert.Equal(mockSchedule.Seats[i].SeatNumber, result.Seats[i].SeatNumber);
                Assert.Equal(mockSchedule.Seats[i].Status, result.Seats[i].Status);
            }
        }

        #endregion

        #region BookSeatAsync Tests

        [Fact]
        public async Task BookSeatAsync_WithValidInput_ReturnsSuccess()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["A1"],
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Booking successful!", result.Message);
            Assert.NotNull(result.TicketId);
            Assert.NotEqual(Guid.Empty, result.TicketId.Value);
        }

        [Fact]
        public async Task BookSeatAsync_WithInvalidScheduleId_ReturnsFailure()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["A1"],
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync((BusSchedule?)null);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Bus schedule not found", result.Message);
            Assert.Null(result.TicketId);
        }

        [Fact]
        public async Task BookSeatAsync_WithUnavailableSeat_ReturnsFailure()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["A2"], // A2 is already Booked
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Seat A2 is not available", result.Message);
            Assert.Null(result.TicketId);
        }

        [Fact]
        public async Task BookSeatAsync_WithNonExistentSeat_ReturnsFailure()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["Z99"], // Non-existent seat
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Seat Z99 is not available", result.Message);
            Assert.Null(result.TicketId);
        }

        [Fact]
        public async Task BookSeatAsync_WithMultipleSeats_UpdatesAllSeatsToBooked()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["A1", "A3"],
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            Assert.True(result.Success);

            // Verify seats were updated to "Booked" status
            var seatA1 = mockSchedule.Seats.First(s => s.SeatNumber == "A1");
            var seatA3 = mockSchedule.Seats.First(s => s.SeatNumber == "A3");
            Assert.Equal("Booked", seatA1.Status);
            Assert.Equal("Booked", seatA3.Status);
        }

        [Fact]
        public async Task BookSeatAsync_CreatesPassengerWithCorrectDetails()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["A1"],
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            _mockPassengerRepo.Verify(x => x.AddPassengerAsync(
                It.Is<Passenger>(p =>
                    p.Name == "John Doe" &&
                    p.MobileNumber == "01711111111" &&
                    p.Id != Guid.Empty)),
                Times.Once);
        }

        [Fact]
        public async Task BookSeatAsync_CreatesTicketWithCorrectDetails()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["A1", "A3"],
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            _mockTicketRepo.Verify(x => x.AddTicketAsync(
                It.Is<Ticket>(t =>
                    t.BusScheduleId == scheduleId &&
                    t.SeatNumber == "A1,A3" &&
                    t.BoardingPoint == "Dhaka" &&
                    t.DroppingPoint == "Chittagong" &&
                    t.Status == "Booked" &&
                    t.Id != Guid.Empty &&
                    t.PassengerId != Guid.Empty)),
                Times.Once);
        }

        [Fact]
        public async Task BookSeatAsync_WithMultipleSeatsOneUnavailable_StopsBeforeUpdating()
        {
            // Arrange
            var scheduleId = Guid.NewGuid();
            var mockSchedule = GetMockBusSchedule(scheduleId);

            var bookingInput = new BookSeatInputDto
            {
                BusScheduleId = scheduleId,
                SeatNumbers = ["A1", "A2"], // A2 is booked
                PassengerName = "John Doe",
                PassengerMobile = "01711111111",
                BoardingPoint = "Dhaka",
                DroppingPoint = "Chittagong"
            };

            _mockBusScheduleRepo
                .Setup(x => x.GetScheduleByIdAsync(scheduleId))
                .ReturnsAsync(mockSchedule!);

            // Act
            var result = await _bookingService.BookSeatAsync(bookingInput);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("not available", result.Message);

            // Verify no passenger or ticket was created
            _mockPassengerRepo.Verify(x => x.AddPassengerAsync(It.IsAny<Passenger>()), Times.Never);
            _mockTicketRepo.Verify(x => x.AddTicketAsync(It.IsAny<Ticket>()), Times.Never);
        }

        #endregion

        private static BusSchedule GetMockBusSchedule(Guid scheduleId)
        {
            var bus = new Bus
            {
                Id = Guid.NewGuid(),
                Name = "Express 101",
                CompanyName = "Green Line",
                TotalSeats = 40
            };

            var route = new Route
            {
                Id = Guid.NewGuid(),
                FromCity = "Dhaka",
                ToCity = "Chittagong"
            };

            return new BusSchedule
            {
                Id = scheduleId,
                BusId = bus.Id,
                Bus = bus,
                RouteId = route.Id,
                Route = route,
                JourneyDate = new DateTime(2025, 11, 1),
                StartTime = "08:00 AM",
                ArrivalTime = "02:00 PM",
                Price = 800,
                Seats =
                [
                    new Seat { Id = Guid.NewGuid(), SeatNumber = "A1", Status = "Available", BusScheduleId = scheduleId },
                    new Seat { Id = Guid.NewGuid(), SeatNumber = "A2", Status = "Booked", BusScheduleId = scheduleId },
                    new Seat { Id = Guid.NewGuid(), SeatNumber = "A3", Status = "Available", BusScheduleId = scheduleId }
                ]
            };
        }
    }
}
using Application.Contracts;
using Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Contracts.DTOs;
using Application.Contracts.Interfaces;

namespace Application.Services
{
    public class BookingService
    {
        private readonly IBusScheduleRepository _busScheduleRepo;
        private readonly ISeatRepository _seatRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly IPassengerRepository _passengerRepo;

        public BookingService(
            IBusScheduleRepository busScheduleRepo,
            ISeatRepository seatRepo,
            ITicketRepository ticketRepo,
            IPassengerRepository passengerRepo)
        {
            _busScheduleRepo = busScheduleRepo;
            _seatRepo = seatRepo;
            _ticketRepo = ticketRepo;
            _passengerRepo = passengerRepo;
        }

        public async Task<SeatPlanDto> GetSeatPlanAsync(Guid busScheduleId)
        {
            var schedule = await _busScheduleRepo.GetScheduleByIdAsync(busScheduleId);
            var seatDtos = schedule.Seats
                .Select(s => new SeatDto
                {
                    SeatNumber = s.SeatNumber,
                    Status = s.Status
                })
                .ToList();

            return new SeatPlanDto { Seats = seatDtos };
        }

        public async Task<BookSeatResultDto> BookSeatAsync(BookSeatInputDto input)
        {
            var schedule = await _busScheduleRepo.GetScheduleByIdAsync(input.BusScheduleId);
            if (schedule == null)
                return new BookSeatResultDto { Success = false, Message = "Bus schedule not found" };

          
            foreach (var seatNum in input.SeatNumbers)
            {
                var seat = schedule.Seats.FirstOrDefault(s => s.SeatNumber == seatNum);
                if (seat == null || seat.Status != "Available")
                    return new BookSeatResultDto
                    {
                        Success = false,
                        Message = $"Seat {seatNum} is not available"
                    };

                seat.Status = "Booked";
                await _seatRepo.UpdateSeatAsync(seat);
            }

            // ✅ Create Passenger
            var passenger = new Passenger
            {
                Id = Guid.NewGuid(),
                Name = input.PassengerName,
                MobileNumber = input.PassengerMobile
            };
            await _passengerRepo.AddPassengerAsync(passenger);

            // ✅ Create Ticket linked with Passenger
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                BusScheduleId = schedule.Id,
                PassengerId = passenger.Id,
                SeatNumber = string.Join(",", input.SeatNumbers),
                BoardingPoint = input.BoardingPoint,
                DroppingPoint = input.DroppingPoint,
                Status = "Booked"
            };

            await _ticketRepo.AddTicketAsync(ticket);

            return new BookSeatResultDto
            {
                Success = true,
                Message = "Booking successful!",
                TicketId = ticket.Id
            };
        }
    }
}


using Application.Contracts.DTOs;
using Application.Contracts.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SearchService
    {
        private readonly IBusScheduleRepository _busScheduleRepository;

        public SearchService(IBusScheduleRepository busScheduleRepository)
        {
            _busScheduleRepository = busScheduleRepository;
        }

        public async Task<List<AvailableBusDto>> SearchAvailableBusesAsync(string from, string to, DateTime journeyDate)
        {
            var schedules = await _busScheduleRepository.GetSchedulesAsync(from, to, journeyDate);

            return schedules.Select(s => new AvailableBusDto
            {
                BusScheduleId = s.Id,
                BusName = s.Bus.Name,
                CompanyName = s.Bus.CompanyName,
                FromCity = s.Route.FromCity,
                ToCity = s.Route.ToCity,
                StartTime = s.StartTime,
                ArrivalTime = s.ArrivalTime,
                Price = s.Price,
                SeatsLeft = s.Seats.Count(seat => seat.Status == "Available")
            }).ToList();
        }
    }
}

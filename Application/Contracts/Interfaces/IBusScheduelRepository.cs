using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;


namespace Application.Contracts.Interfaces
{
    public interface IBusScheduleRepository
    {
        Task<List<BusSchedule>> GetSchedulesAsync(string fromCity, string toCity, DateTime journeyDate);
        Task<BusSchedule> GetScheduleByIdAsync(Guid busScheduleId);
    }
}


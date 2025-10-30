using Application.Contracts;
using Application.Contracts.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BusScheduleRepository : IBusScheduleRepository
    {
        private readonly AppDbContext _context;

        public BusScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BusSchedule?> GetScheduleByIdAsync(Guid id)
        {
            return await _context.BusSchedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<BusSchedule>> GetSchedulesAsync(string fromCity, string toCity, DateTime journeyDate)
        {
            return await _context.BusSchedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .Include(s => s.Seats)
                .Where(s => s.Route.FromCity.ToLower() == fromCity.ToLower() &&
                            s.Route.ToCity.ToLower() == toCity.ToLower() &&
                            s.JourneyDate.Date == journeyDate.Date)
                .ToListAsync();
        }
    }
}

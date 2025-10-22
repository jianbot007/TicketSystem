using Application.Contracts;
using Application.Contracts.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;

        public SeatRepository(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<Seat?> GetSeatAsync(Guid busScheduleId, string seatNumber)
        {
            return await _context.Seats
                .FirstOrDefaultAsync(s => s.BusScheduleId == busScheduleId && s.SeatNumber == seatNumber);
        }

       
        public async Task UpdateSeatAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Seat>> GetSeatsByScheduleAsync(Guid busScheduleId)
        {
            return await _context.Seats
                .Where(s => s.BusScheduleId == busScheduleId)
                .ToListAsync();
        }
    }
}

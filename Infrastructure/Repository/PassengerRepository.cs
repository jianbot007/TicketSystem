using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;
using Application.Contracts.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly AppDbContext _context;

        public PassengerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPassengerAsync(Passenger passenger)
        {
            await _context.Passengers.AddAsync(passenger);
            await _context.SaveChangesAsync();
        }

        public async Task<Passenger?> GetPassengerByIdAsync(Guid id)
        {
            return await _context.Passengers.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}

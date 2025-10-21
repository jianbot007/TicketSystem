using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;


namespace Application.Contracts.Interfaces
{
    public interface ISeatRepository
    {
        Task<Seat> GetSeatAsync(Guid busScheduleId, string seatNumber);
        Task UpdateSeatAsync(Seat seat);
    }
}


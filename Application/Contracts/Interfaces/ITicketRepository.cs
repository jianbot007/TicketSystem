using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Contracts.Interfaces
{
    public interface ITicketRepository
    {
        Task AddTicketAsync(Ticket ticket);
    }
}


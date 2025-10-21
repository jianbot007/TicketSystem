using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Contracts.DTOs
{
    public class SeatPlanDto
    {
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
    }

    public class SeatDto
    {
        public string SeatNumber { get; set; }
        public string Status { get; set; } // "Available", "Booked", "Sold"
    }
}



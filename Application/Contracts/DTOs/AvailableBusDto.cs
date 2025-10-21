using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Contracts.DTOs
{
    public class AvailableBusDto
    {
        public Guid BusScheduleId { get; set; }
        public string BusName { get; set; }
        public string CompanyName { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public string StartTime { get; set; }
        public string ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public int SeatsLeft { get; set; }
    }
}

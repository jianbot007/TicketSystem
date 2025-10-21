using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Contracts.DTOs
{
    public class BookSeatInputDto
    {
        public Guid BusScheduleId { get; set; }
        public List<string> SeatNumbers { get; set; } = new List<string>();
        public string PassengerName { get; set; }
        public string PassengerMobile { get; set; }
        public string BoardingPoint { get; set; }
        public string DroppingPoint { get; set; }
    }
}

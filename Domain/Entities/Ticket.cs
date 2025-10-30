using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }

        public Guid BusScheduleId { get; set; }
        public BusSchedule BusSchedule { get; set; }

        public string SeatNumber { get; set; }

        public Guid PassengerId { get; set; }
        public Passenger Passenger { get; set; }

        public string BoardingPoint { get; set; }
        public string DroppingPoint { get; set; }

        public string Status { get; set; } // "Booked", "Confirmed"
    }
}


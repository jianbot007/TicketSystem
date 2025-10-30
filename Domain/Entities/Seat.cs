using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    using System;

    namespace Domain.Entities
    {
        public class Seat
        {
            public Guid Id { get; set; }
            public string SeatNumber { get; set; }
            public string Status { get; set; } // "Available", "Booked"

            public Guid BusScheduleId { get; set; } // foreign key
            public BusSchedule BusSchedule { get; set; } 
        }
    }



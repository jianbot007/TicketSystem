using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Bus
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public int TotalSeats { get; set; }

        // EF: One bus can have many schedules
        public List<BusSchedule> Schedules { get; set; } = new List<BusSchedule>();
    }
}



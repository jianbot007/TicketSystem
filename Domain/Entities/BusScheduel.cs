using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class BusSchedule
    {
        public Guid Id { get; set; }

        public Guid BusId { get; set; }      
        public Bus Bus { get; set; }        

        public Guid RouteId { get; set; }    
        public Route Route { get; set; }     

        public DateTime JourneyDate { get; set; }
        public string StartTime { get; set; }   
        public string ArrivalTime { get; set; } 
        public decimal Price { get; set; }

       
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}


using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            if (await context.Buses.AnyAsync())
                return;

            var buses = new List<Bus>();
            var routes = new List<Route>();
            var schedules = new List<BusSchedule>();
            var seats = new List<Seat>();

            // --- Generate 200 Buses ---
            for (int i = 1; i <= 200; i++)
            {
                var bus = new Bus
                {
                    Id = Guid.NewGuid(),
                    Name = $"Bus-{i}",
                    CompanyName = $"Company-{(i % 10) + 1}",
                    TotalSeats = 40
                };
                buses.Add(bus);
            }

            await context.Buses.AddRangeAsync(buses);

            // --- Generate 200 Routes ---
            string[] cities = { "Dhaka", "Chittagong", "Sylhet", "Khulna", "Rajshahi", "Barishal", "Rangpur", "Cox's Bazar" };
            for (int i = 1; i <= 200; i++)
            {
                var route = new Route
                {
                    Id = Guid.NewGuid(),
                    FromCity = cities[i % cities.Length],
                    ToCity = cities[(i + 1) % cities.Length]
                };
                routes.Add(route);
            }

            await context.Routes.AddRangeAsync(routes);

            // --- Generate 200 BusSchedules ---
            var random = new Random();
            for (int i = 0; i < 200; i++)
            {
                var schedule = new BusSchedule
                {
                    Id = Guid.NewGuid(),
                    Bus = buses[i],
                    Route = routes[i],
                  
                    JourneyDate = DateTime.UtcNow.Date.AddDays(random.Next(0, 10)),
                    StartTime = $"{random.Next(5, 22):D2}:00",
                    ArrivalTime = $"{random.Next(5, 22):D2}:00",
                    Price = random.Next(400, 800)
                };
                schedules.Add(schedule);
            }

            await context.BusSchedules.AddRangeAsync(schedules);

          
            foreach (var schedule in schedules)
            {
                schedule.Seats = new List<Seat>();
                for (int j = 1; j <= schedule.Bus.TotalSeats; j++)
                {
                    seats.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        SeatNumber = $"S{j:D2}",
                        Status = "Available",
                        BusSchedule = schedule
                    });
                }
            }

            await context.Seats.AddRangeAsync(seats);

            await context.SaveChangesAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<BusSchedule> BusSchedules { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1 Bus → Many Schedules
            modelBuilder.Entity<Bus>()
                .HasMany(b => b.Schedules)
                .WithOne(s => s.Bus)
                .HasForeignKey(s => s.BusId);

            // 1 Route → Many Schedules
            modelBuilder.Entity<Route>()
                .HasMany<BusSchedule>()
                .WithOne(s => s.Route)
                .HasForeignKey(s => s.RouteId);

            // 1 Schedule → Many Seats
            modelBuilder.Entity<BusSchedule>()
                .HasMany(s => s.Seats)
                .WithOne(seat => seat.BusSchedule)
                .HasForeignKey(seat => seat.BusScheduleId);

            // 1 Passenger → Many Tickets
            modelBuilder.Entity<Passenger>()
                .HasMany<Ticket>()
                .WithOne(t => t.Passenger)
                .HasForeignKey(t => t.PassengerId);

            // 1 Schedule → Many Tickets
            modelBuilder.Entity<BusSchedule>()
                .HasMany<Ticket>()
                .WithOne(t => t.BusSchedule)
                .HasForeignKey(t => t.BusScheduleId);
        }
    }
}


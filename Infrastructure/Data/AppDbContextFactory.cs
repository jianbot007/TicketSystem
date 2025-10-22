using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Use your PostgreSQL connection string
            optionsBuilder.UseNpgsql("Host=localhost;Database=MyAppDb;Username=postgres;Password=newpassword");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

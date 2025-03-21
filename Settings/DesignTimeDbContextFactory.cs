using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// chatgpt code 
namespace TicketingSys.Settings
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Load configuration from appsettings.json or another source
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Makes sure to set the correct directory
                .AddJsonFile("appsettings.json") // Your configuration file
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString); // Ensure you're using PostgreSQL

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

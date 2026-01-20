using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Booking.DataAccess.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
{
    public BookingDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<BookingDbContext>();
            
        var connectionString = configuration.GetConnectionString("DefaultConnection");
            
        builder.UseNpgsql(connectionString); 

        return new BookingDbContext(builder.Options);
    }
}
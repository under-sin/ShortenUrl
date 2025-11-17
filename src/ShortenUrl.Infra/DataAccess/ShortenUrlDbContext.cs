using Microsoft.EntityFrameworkCore;
using ShortenUrl.Domain.Entities;

namespace ShortenUrl.Infra.DataAccess;

public class ShortenUrlDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Url> Urls { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShortenUrlDbContext).Assembly);
    }
}
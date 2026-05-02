using Microsoft.EntityFrameworkCore;
using ShortLink.Models;

namespace ShortLink.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UrlEntry> UrlEntries { get; set; }
}
namespace Infraestructure.DbContexts;

using Application.Models;
using Microsoft.EntityFrameworkCore;

public class CloudGalleryDbContext : DbContext
{
    public CloudGalleryDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Photo> Photos { get; set; } = null!;
}
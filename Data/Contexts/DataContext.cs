using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<SubscribeEntity> Subscribes { get; set; } = null!;
    }
}

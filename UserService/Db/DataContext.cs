using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Db;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
}
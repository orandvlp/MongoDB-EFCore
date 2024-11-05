using Microsoft.EntityFrameworkCore;
using TestMongoEfCore.DbModel;

namespace TestMongoEfCore;

public class MyDbContext(DbContextOptions<MyDbContext> options)
    : DbContext(options)
{
    public DbSet<OrderDbModel> Orders { get; set; }
}
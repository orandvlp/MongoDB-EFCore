using Microsoft.EntityFrameworkCore;
using TestMongoEfCore.DbModel;

namespace TestMongoEfCore;

public class MyDbContext(DbContextOptions<MyDbContext> options)
    : DbContext(options)
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<Post>(
        //     b =>
        //     {
        //         b.Property(e => e.Title);
        //     });
        base.OnModelCreating(modelBuilder);
    }
}
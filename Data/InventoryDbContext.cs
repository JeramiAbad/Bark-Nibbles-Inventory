using Barks.Models;
using Microsoft.EntityFrameworkCore;

namespace Barks.Data;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<AdminAccount> AdminAccounts { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<Transaction> Transactions { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>()
            .HasIndex(product => product.ItemCode)
            .IsUnique();
        modelBuilder.Entity<AdminAccount>()
            .Property(account => account.Username)
            .UseCollation("NOCASE");
    }
}

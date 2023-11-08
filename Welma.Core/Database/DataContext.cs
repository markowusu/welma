using Microsoft.EntityFrameworkCore;
using WelmaTransactions.Models;

namespace Welma.Core.Database;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration){
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options){
        options.UseNpgsql(Configuration.GetConnectionString("Postgres"));
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Wallet> Wallets { get; set; }

    // creating a relationship between the User and Transaction ... This relationship specifies that the User has a one to many relationship with the transaction 
    // the user can have a List[transactions]

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Transaction>()
        .HasOne(t => t.User)        // Transaction has one User
        .WithMany(u => u.Transactions)  // User can have many Transactions
        .HasForeignKey(t => t.UserId); // Foreign key property in Transaction

    modelBuilder.Entity<Wallet>()
            .HasOne(w => w.User) // Wallet have just one user 
            .WithMany(u => u.Wallets) // User can have many wallets
            .HasForeignKey(w => w.UserId);

    modelBuilder.Entity<User>().Property(u => u.Id).HasConversion<Guid>();

    modelBuilder.Entity<Wallet>().Property(w => w.Id).HasConversion<Guid>();

    modelBuilder.Entity<Transaction>().Property(t => t.Id).HasConversion<Guid>();
}

}
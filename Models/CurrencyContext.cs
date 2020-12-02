using Microsoft.EntityFrameworkCore;

namespace CurrencyTest.Models
{
  public class CurrencyContext : DbContext, ICurrencyContext
  {
    public CurrencyContext(DbContextOptions<CurrencyContext> options)
      : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
  }

  public interface ICurrencyContext  {

  }
}
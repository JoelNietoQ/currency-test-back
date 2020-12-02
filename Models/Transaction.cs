using System;

namespace CurrencyTest.Models
{
  public class Transaction
  {
    public Transaction()
    {
      Date = DateTime.Now;
    }

    public int Id { get; set; }
    public User User { get; set; }
    public string Currency { get; set; }
    public double Amount { get; set; }
    public double Result { get; set; }
    public DateTime Date { get; set; }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyTest.Models;

namespace CurrencyTest.Services
{
  public class CurrenciesService : ICurrenciesService
  {
    private readonly HttpClient http = new HttpClient();
    private readonly CurrencyContext _context;

    public CurrenciesService(CurrencyContext context)
    {
      _context = context;
    }

    public async Task<string[]> GetExchange(string currency)
    {
      HttpResponseMessage response = await http.GetAsync("https://www.bancoprovincia.com.ar/Principal/dolar");
      response.EnsureSuccessStatusCode();
      string body = await response.Content.ReadAsStringAsync();
      var res = JsonSerializer.Deserialize<string[]>(body);
      if (currency == "BRL")
      {
        res[0] = (double.Parse(res[0]) * .25).ToString();
        res[1] = (double.Parse(res[1]) * .25).ToString();
      }

      return res;
    }

    public IEnumerable<Transaction> GetTransactions()
    {
      return _context.Transactions.OrderBy(x => x.Date);
    }

    public bool ValidateCurrency(string currency)
    {
      currency = currency.ToUpper();
      string[] currencies = { "USD", "BRL" };
      return currencies.Any(currency.Contains);
    }

    public async Task<Transaction> CurrencyPurchase(Purchase purchase)
    {
      string[] exchange = await GetExchange(purchase.Currency);

      var user = _context.Users.Find(purchase.UserId);
      var transaction = new Transaction() { User = user, Amount = purchase.Amount, Currency = purchase.Currency };
      transaction.Result = purchase.Amount * Double.Parse(exchange[1]);
      _context.Add(transaction);
      _context.SaveChanges();
      return transaction;
    }
  }

  public interface ICurrenciesService
  {
    IEnumerable<Transaction> GetTransactions();
    Task<string[]> GetExchange(string currency);
    Task<Transaction> CurrencyPurchase(Purchase purchase);
    bool ValidateCurrency(string currency);
  }

  public class Purchase
  {
    public int UserId { get; set; }
    public double Amount { get; set; }
    public string Currency { get; set; }
  }

}
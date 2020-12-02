using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyTest.Models;
using CurrencyTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTest.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  public class CurrencyController : ControllerBase
  {
    private readonly HttpClient http = new HttpClient();
    private readonly ICurrenciesService _context;

    public CurrencyController(ICurrenciesService context)
    {
      _context = context;
    }

    [HttpGet("{currency}")]
    public async Task<IActionResult> GetExchange(string currency)
    {
      if (_context.ValidateCurrency(currency))
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
        return Ok(res);
      }
      else
      {
        return NotFound("Currency not found!");
      }
    }

    [HttpGet("Transactions")]
    public IEnumerable<Transaction> GetTransactions()
    {
      return _context.GetTransactions();
    }

    [HttpPost("Purchase")]
    public async Task<IActionResult> CurrencyPurchase(Purchase purchase)
    {
      purchase.Currency = purchase.Currency.ToUpper();
      string[] currencies = { "USD", "BRL" };
      if (_context.ValidateCurrency(purchase.Currency))
      {
        try
        {
          var transaction = await (_context.CurrencyPurchase(purchase));
          return Ok(transaction);
        }
        catch (System.Exception e)
        {
          return BadRequest(e);
          throw;
        }
      }
      else
      {
        return BadRequest("Currency not found!");
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coinbase.Exceptions;
using Coinbase.Models;
using Microsoft.EntityFrameworkCore;

namespace Coinbase.Repositories
{
    public class CryptoCurrencyRepository: ICryptocurrencyRepository
    {
        public readonly DatabaseContext _database;

        public CryptoCurrencyRepository(DatabaseContext database)
        {
            _database = database;
        }
        
        public async Task<IEnumerable<Cryptocurrency>> All()
        {
            return await _database.Cryptocurrencies.ToListAsync();
        }

        public async Task<Cryptocurrency> Get(int rank)
        {
            Cryptocurrency model = await _database.Cryptocurrencies.FirstOrDefaultAsync(cryptocurrency => cryptocurrency.Rank == rank);

            if (model is null)
                throw new UserErrorException($"No cryptocurrency found for the id {rank}");
            
            return model;
        }

        public async Task<Cryptocurrency> Create(Cryptocurrency cryptocurrency)
        {
            if (cryptocurrency is null)
                throw new UserErrorException("cryptocurrency cannot be empty");

            if (cryptocurrency.Name == "")
                throw new Exception("Name cannot be empty");
            
            await _database.Cryptocurrencies.AddAsync(cryptocurrency);
            await _database.SaveChangesAsync();
            return cryptocurrency;
        }

        public async Task<Cryptocurrency> Update(Cryptocurrency cryptocurrency)
        {
            await Get(cryptocurrency.Rank);

            _database.Cryptocurrencies.Update(cryptocurrency);
            await _database.SaveChangesAsync();
            return cryptocurrency;
        }

        public async void Delete(int rank)
        {
            Cryptocurrency cryptocurrency = await Get(rank);
            _database.Cryptocurrencies.Remove(cryptocurrency);
            await _database.SaveChangesAsync();
        }

        public async Task<IEnumerable<CoinMarketCap>> MarketCap()
        {
            //throw new System.NotImplementedException();
            
            List<CoinMarketCap> markets = new List<CoinMarketCap>();
            
            //loop through each of the currencies
            foreach (Cryptocurrency currency in _database.Cryptocurrencies)
                markets.Add(new CoinMarketCap {Name = currency.Name, MarketCap = currency.MarketCap, AvailableSupply = currency.AvailableSupply});
            return markets;
        }

        public async Task<IEnumerable<Cryptocurrency>> Search(string name)
        {
            name = name.ToLower();

            return await _database.Cryptocurrencies
                .Where(cryptocurrency => cryptocurrency.Name.ToLower().Contains(name)).ToListAsync();
        }

        public async Task<IEnumerable<Cryptocurrency>> PriceRange(double min, double max)
        {
            List<Cryptocurrency> cryptocurrencies = new List<Cryptocurrency>();
            //loop through each of the currencies
            foreach (Cryptocurrency currency in _database.Cryptocurrencies)
            {
                //strip out the $ and , from the price string
                currency.Price = currency.Price.Replace("$", "").Replace(",", "");
                //convert the price to double so we can use it for range comparing
                double price = double.Parse(currency.Price);
                
                //if the price is in the range, we add it to the list
                if (price >= min && price <= max)
                {
                    cryptocurrencies.Add(currency);
                }
            }

            return cryptocurrencies;
        }
    }
}
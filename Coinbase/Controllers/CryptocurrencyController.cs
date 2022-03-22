using System.Threading.Tasks;
using Coinbase.Models;
using Coinbase.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coinbase.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cryptocurrency")]
    public class CryptocurrencyController : ControllerBase
    {
        private readonly ICryptocurrencyRepository _repository;

        public CryptocurrencyController(ICryptocurrencyRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> AllCryptocurrencies()
        {
            return Ok(await _repository.All());
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(Cryptocurrency cryptocurrency)
        {
            return Ok(await _repository.Create(cryptocurrency));
        }
        
        [HttpPut]
        public async Task<IActionResult> UpdateCryptocurrency(Cryptocurrency cryptocurrency)
        {
            return Ok(await _repository.Update(cryptocurrency));
        }
        
        [HttpGet("{rank}")]
        public async Task<IActionResult> GetCryptocurrencyByRank(int rank)
        {
            return Ok(await _repository.Get(rank));
        }
        
        [HttpDelete("{rank}")]
        public async Task<IActionResult> DeleteFilm(int rank)
        {
            _repository.Delete(rank);
            return Ok("Cryptocurrency successful deleted");
        }
        
        [HttpGet("marketcap")]
        public async Task<IActionResult> MarketCap()
        {
            return Ok(await _repository.MarketCap());
        }
        
        [HttpGet("search/name/{keyword}")]
        public async Task<IActionResult> SearchByName(string keyword)
        {
            return Ok(await _repository.Search(keyword));
        }
        
        [HttpGet("pricerange/{start}/{end}")]
        public async Task<IActionResult> PriceRange(double min, double max)
        {
            return Ok(await _repository.PriceRange(min, max));
        }
    }
}
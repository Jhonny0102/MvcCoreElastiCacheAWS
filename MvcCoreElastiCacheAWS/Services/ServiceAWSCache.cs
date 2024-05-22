using Microsoft.Extensions.Caching.Distributed;
using MvcCoreElastiCacheAWS.Models;
using Newtonsoft.Json;
using System.Text;

namespace MvcCoreElastiCacheAWS.Services
{
    public class ServiceAWSCache
    {
        private readonly IDistributedCache _cache;

        public ServiceAWSCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<List<Coche>> GetCochesFavoritosAsync()
        {
            var jsonCoches = await _cache.GetStringAsync("cochesfavoritos");
            if (jsonCoches == null)
            {
                return null;
            }
            else
            {
                List<Coche> cars = JsonConvert.DeserializeObject<List<Coche>>(jsonCoches);
                return cars;
            }
        }

        public async Task AddCochefavoritoAsync(Coche car)
        {
            var coches = await GetCochesFavoritosAsync();
            if (coches == null)
            {
                coches = new List<Coche>();
            }
            coches.Add(car);

            var jsonCoches = JsonConvert.SerializeObject(coches);
            await _cache.SetStringAsync("cochesfavoritos", jsonCoches, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
        }

        public async Task DeleteCocheFavoritoAsync(int idcoche)
        {
            var cars = await GetCochesFavoritosAsync();
            if (cars != null)
            {
                var cocheEliminar = cars.FirstOrDefault(z => z.IdCoche == idcoche);
                if (cocheEliminar != null)
                {
                    cars.Remove(cocheEliminar);

                    if (cars.Count == 0)
                    {
                        await _cache.RemoveAsync("cochesfavoritos");
                    }
                    else
                    {
                        var jsonCoches = JsonConvert.SerializeObject(cars);
                        await _cache.SetStringAsync("cochesfavoritos", jsonCoches, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                        });
                    }
                }
            }
        }
    }
}

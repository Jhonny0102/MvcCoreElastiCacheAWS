using MvcCoreElastiCacheAWS.Helpers;
using MvcCoreElastiCacheAWS.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MvcCoreElastiCacheAWS.Services
{
    public class ServiceAWSCache
    {
        private IDatabase cache;

        public ServiceAWSCache()
        {
            this.cache = HelperCacheRedis.Connection.GetDatabase();
        }

        public async Task<List<Coche>> GetCochesFavoritosAsync()
        {
            string jsonCoches = await this.cache.StringGetAsync("cochesfavoritos");
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
            List<Coche> coches = await this.GetCochesFavoritosAsync();
            //SINO EXISTEN COCHES FAVORITOS TODAVIA , CREAMOS LA COLECCCION
            if (coches == null)
            {
                coches = new List<Coche>();
            }
            else
            {
                //AÑADIMOS EL NUEVO COCHE A LA COLECCION
                coches.Add(car);
                //SERIALIZAMOS A JSON LA COLECCION
                string jsonCoches = JsonConvert.SerializeObject(coches);
                //ALAMCENAMOS LA COLECCION DENTRO DE CACHE REDIS
                //INDICAREMOS QUE LOS DATOS DURARAN 30 MINUTOS
                await this.cache.StringSetAsync("cochesfavoritos", jsonCoches, TimeSpan.FromMinutes(30));
            }
        }

        public async Task DeleteCocheFavoritoAsync(int idcoche)
        {
            List<Coche> cars = await this.GetCochesFavoritosAsync();
            if (cars != null)
            {
                Coche cocheEliminar = cars.FirstOrDefault(z => z.IdCoche == idcoche);
                cars.Remove(cocheEliminar);
                //COMPROBAMOS SI LA COLECCION TIENE COCHES FAVORITOS TODAVIA O NO TIENE
                //SINO TIENE COCHES, ELIMINAMOS LA KEY DE CACHE REDIS
                if (cars.Count == 0)
                {
                    await this.cache.KeyDeleteAsync("cochesfavoritos");
                }
                else
                {
                    //ALMACENAMOS DE NUEVO LOS COCHES SIN EL CAR ELIMINADO
                    string jsonCoches = JsonConvert.SerializeObject(cars);
                    //ACTUALIZAMOS EL CACHE REDIS
                    await this.cache.StringSetAsync("cochesfavoritos" , jsonCoches , TimeSpan.FromMinutes(30));
                }
            }
        }
    }
}

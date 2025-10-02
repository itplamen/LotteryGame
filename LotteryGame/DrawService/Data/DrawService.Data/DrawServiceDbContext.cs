namespace DrawService.Data
{
    using System;
    using System.Threading.Tasks;

    using CouchDB.Driver;

    using Microsoft.Extensions.Configuration;

    using DrawService.Data.Models;
    
    public class DrawServiceDbContext : IAsyncDisposable
    {
        private readonly CouchClient client;

        public DrawServiceDbContext(IConfiguration configuration)
        {
            client = new CouchClient(x =>
            {
                x.UseEndpoint(new Uri(configuration["ConnectionString"]));
                x.UseBasicAuthentication(configuration["DB:Username"], configuration["DB:Password"]);
            });
        }

        public async Task<ICouchDatabase<TEntity>> GetDatabase<TEntity>()
            where TEntity : BaseEntity
        {
            return await client.GetOrCreateDatabaseAsync<TEntity>(typeof(TEntity).Name);
        }

        public async ValueTask DisposeAsync()
        {
            if (client != null)
            {
                await client.DisposeAsync();
            }
        } 
    }
}

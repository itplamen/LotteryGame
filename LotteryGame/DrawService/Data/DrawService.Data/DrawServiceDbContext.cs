namespace DrawService.Data
{
    using System;
    using System.Threading.Tasks;

    using CouchDB.Driver;
    using CouchDB.Driver.Options;

    using Microsoft.Extensions.Configuration;

    using DrawService.Data.Models;

    public class DrawServiceDbContext : CouchContext
    {
        private readonly CouchClient client;

        public DrawServiceDbContext(IConfiguration configuration)
        {
            client = new CouchClient(x =>
            {
                x.UseEndpoint(new Uri(configuration["CouchDB:Endpoint"]));
                x.UseBasicAuthentication(configuration["CouchDB:Username"], configuration["CouchDB:Password"]);
            });
        }

        public async Task<ICouchDatabase<TEntity>> GetDatabase<TEntity>()
            where TEntity : BaseEntity
        {
            string dbName = typeof(TEntity).Name.ToLower();

            return await client.GetOrCreateDatabaseAsync<TEntity>(dbName);
        }

        protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
        {
            optionsBuilder
              .UseEndpoint("http://localhost:5984/")
              .EnsureDatabaseExists()
              .UseBasicAuthentication(username: "admin", password: "admin");

            base.OnConfiguring(optionsBuilder);
        }
    }
}

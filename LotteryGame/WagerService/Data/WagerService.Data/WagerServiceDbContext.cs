namespace WagerService.Data
{
    using Microsoft.Extensions.Configuration;
    
    using MongoDB.Driver;

    using WagerService.Data.Models;

    public class WagerServiceDbContext
    {
        private readonly IMongoDatabase database;

        public WagerServiceDbContext(IConfiguration config)
        {
            string dbHost = config["MongoDb:Host"];
            string dbName = config["MongoDb:DatabaseName"];
            database = CreateDatabase(dbHost, dbName);
        }

        public void Init()
        {
            InitCollection(
                Builders<Ticket>.IndexKeys.Ascending(x => x.TicketNumber),
                new CreateIndexOptions { Unique = true });

            InitCollection(
                Builders<Ticket>.IndexKeys.Ascending(x => x.ReservationId));
            
            InitCollection(
                Builders<Ticket>.IndexKeys.Ascending(x => x.DrawId));
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>()
            where TEntity : BaseEntity
        {
            return GetOrCreateCollection<TEntity>();
        }

        private IMongoDatabase CreateDatabase(string host, string dbName)
        {
            IMongoClient mongoClient = new MongoClient(host);
            return mongoClient.GetDatabase(dbName);
        }

        private IMongoCollection<TEntity> GetOrCreateCollection<TEntity>()
            where TEntity : BaseEntity
        {
            return database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        private IMongoCollection<TEntity> InitCollection<TEntity>(IndexKeysDefinition<TEntity> keys = null, CreateIndexOptions options = null)
            where TEntity : BaseEntity
        {
            IMongoCollection<TEntity> collection = GetOrCreateCollection<TEntity>();

            if (keys != null)
            {
                var indexModel = new CreateIndexModel<TEntity>(keys, options);
                collection.Indexes.CreateOne(indexModel);
            }

            return collection;
        }
    }
}

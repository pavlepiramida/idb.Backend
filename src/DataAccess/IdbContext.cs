using idb.Backend.Providers;
using MongoDB.Driver;

namespace idb.Backend.DataAccess
{
    public interface IidbContext
    {
        IMongoCollection<TEntity> GetCollection<TEntity>(string name);
    }
    public class IdbContext : IidbContext
    {
        private IMongoDatabase _db { get; set; }
        private IMongoClient _mongoClient { get; set; }

        public IdbContext(IMongoClient mongoClient,IDatabaseEnvironmentProvider dbEnvProvider)
        {
            _mongoClient = mongoClient;
            _db = _mongoClient.GetDatabase(dbEnvProvider.Database);

        }
        public IMongoCollection<TEntity> GetCollection<TEntity>(string name)
        {
            return _db.GetCollection<TEntity>(name);
        }
    }
}

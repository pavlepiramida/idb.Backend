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
        private MongoClient _mongoClient { get; set; }
        public IClientSessionHandle Session { get; set; }
        public IdbContext(string connection, string databaseName)
        {
            var settings = MongoClientSettings.FromConnectionString(connection);
            _mongoClient = new MongoClient(settings);
            _db = _mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>(string name)
        {
            return _db.GetCollection<TEntity>(name);
        }
    }
}

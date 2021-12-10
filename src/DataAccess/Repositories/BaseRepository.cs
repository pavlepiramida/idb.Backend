using idb.Backend.DataAccess.Models;
using idb.Backend.Providers;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace idb.Backend.DataAccess.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : MongoEntity
    {
        Task<TEntity> Create(TEntity obj);
        Task<TEntity> Update(TEntity obj);
        Task Delete(string id);
        Task<int> IncrementValue();
        Task<TEntity> Get(string id);
        Task<List<TEntity>> Get();
    }
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : MongoEntity
    {
        protected readonly IdbContext _mongoContext;
        protected IMongoCollection<TEntity> _dbCollection;
        private protected IDateTimeProvider _dateTimeProvider;

        protected BaseRepository(IdbContext context, IDateTimeProvider dateTimeProvider)
        {
            _mongoContext = context;
            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
            _dateTimeProvider = dateTimeProvider;
        }
        protected BaseRepository() { }
        public async Task<TEntity> Create(TEntity obj)
        {
            var timestamp = _dateTimeProvider.UtcNow; // lel like Im ever gonna test this
            obj.created_at = timestamp;
            obj.timestamp = timestamp;
            obj.ID = await IncrementValue();
            await _dbCollection.InsertOneAsync(obj);
            return obj;
        }

        public async Task Delete(string id)
        {
            var objectId = new ObjectId(id);
            await _dbCollection.FindOneAndDeleteAsync(Builders<TEntity>.Filter.Eq("_id", objectId));
        }

        public async Task<TEntity> Get(string id)
        {
            var objectId = new ObjectId(id);

            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            var results = await _dbCollection.FindAsync(filter);
            return await results.FirstOrDefaultAsync(); // BIG o o f needs to change but to lazy nau

        }
        public async Task<List<TEntity>> Get()
        {
            var all = await _dbCollection.FindAsync(Builders<TEntity>.Filter.Empty);
            return await all.ToListAsync();
        }

        public async Task<int> IncrementValue()
        {
            var sort = Builders<TEntity>.Sort.Descending("ID");
            var maxdoc = await _dbCollection.Find(Builders<TEntity>.Filter.Empty).Sort(sort).Limit(1).FirstOrDefaultAsync();
            return maxdoc is not null ? 1 + maxdoc.ID : 1;
        }

        public async Task<TEntity> Update(TEntity obj)
        {
            obj.timestamp = _dateTimeProvider.UtcNow;
            var objectId = new ObjectId(obj.guid);
            await _dbCollection.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", objectId), obj);
            return obj;
        }
    }
}

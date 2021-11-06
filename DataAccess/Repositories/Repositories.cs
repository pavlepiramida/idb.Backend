using idb.Backend.DataAccess.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace idb.Backend.DataAccess.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByIDThatIWillDeleteSoon(int id);
    }
    public interface IItemRepository : IBaseRepository<Item>
    {
        Task<List<Item>> GetBy(string search, List<int> tag_ids, string userId);
    }

    public interface ITagRepository : IBaseRepository<Models.Tag>
    {
        Task<Models.Tag> GetById(int tagId);
        Task<List<Models.Tag>> GetByIds(List<int> tagIds);
    }

    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(idbContext context) : base(context) { }

        public async Task<User> GetByEmail(string email)
        {
            var filter = Builders<User>.Filter.Eq("email", email);
            var response = await _dbCollection.FindAsync(filter);
            return await response.FirstOrDefaultAsync();
        }

        public async Task<User> GetByIDThatIWillDeleteSoon(int id)
        {
            var filter = Builders<User>.Filter.Eq("ID", id);
            var response = await _dbCollection.FindAsync(filter);
            return await response.FirstOrDefaultAsync();
        }
    }

    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(idbContext context) : base(context) { }

        public async Task<List<Item>> GetBy(string search, List<int> tag_ids, string userId)
        {
            FilterDefinition<Item> searchQuery = Builders<Item>.Filter.Empty;
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrWhiteSpace(userId))
                searchQuery = Builders<Item>.Filter.And(searchQuery, Builders<Item>.Filter.Eq(x => x.ownerId, userId));

            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
                searchQuery = Builders<Item>.Filter.And(searchQuery, Builders<Item>.Filter.Where(x => x.content.ToLower().Contains(search))
                    | Builders<Item>.Filter.Where(x => x.name.ToLower().Contains(search)));

            if (tag_ids.Count > 0)
                searchQuery = Builders<Item>.Filter.And(searchQuery, Builders<Item>.Filter.ElemMatch(x => x.tags, Builders<Models.Tag>.Filter.In(x => x.ID, tag_ids)));

            var response = await _dbCollection.FindAsync(searchQuery);
            return await response.ToListAsync();
        }
    }

    public class TagRepository : BaseRepository<Models.Tag>, ITagRepository
    {
        public TagRepository(idbContext context) : base(context) { }

        public async Task<Models.Tag> GetById(int tagId)
        {
            FilterDefinition<Models.Tag> filter = Builders<Models.Tag>.Filter.Eq("ID", tagId);
            var response = await _dbCollection.FindAsync(filter);
            return await response.FirstOrDefaultAsync();
        }

        public async Task<List<Models.Tag>> GetByIds(List<int> tagIds)
        {
            var tagsQuery = Builders<Models.Tag>.Filter.Where(x => tagIds.Contains(x.ID));
            var response = await _dbCollection.FindAsync(tagsQuery);
            return await response.ToListAsync();
        }
    }
}

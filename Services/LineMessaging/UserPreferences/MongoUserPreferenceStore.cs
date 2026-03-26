using WeatherBot.Models;
using MongoDB.Driver;

namespace WeatherBot.Services.LineMessaging.UserPreferences
{
    public class MongoUserPreferenceStore : IUserPreferenceStore
    {
        private readonly IMongoCollection<UserPreference> _collection;

        public MongoUserPreferenceStore(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDb:ConnectionString"];
            var databaseName = configuration["MongoDb:DatabaseName"];
            var collectionName = configuration["MongoDb:UserPreferenceCollectionName"];

            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(collectionName))
            {
                throw new InvalidOperationException("MongoDb settings are not configured correctly.");
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<UserPreference>(collectionName);
        }

        public async Task<UserPreference> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var doc = await _collection.Find(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
            return doc == null
                ? null
                : new UserPreference
                {
                    UserId = doc.UserId,
                    Location = doc.Location,
                };
        }

        public async Task SetLocationAsync(string userId, string location, CancellationToken cancellationToken = default)
        {
            var update = Builders<UserPreference>.Update
                .Set(x => x.Location, location)
                .SetOnInsert(x => x.UserId, userId);

            await _collection.UpdateOneAsync(
                x => x.UserId == userId,
                update,
                new UpdateOptions { IsUpsert = true },
                cancellationToken);
        }

    }
}

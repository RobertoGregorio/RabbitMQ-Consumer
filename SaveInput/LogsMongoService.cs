using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveInput
{
    public class LogsMongoService
    {
        private readonly IMongoCollection<Log> _logsCollection;

        public LogsMongoService(IOptions<MongoDatabaseSettings> MongoDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                MongoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                MongoDatabaseSettings.Value.DatabaseName);

            _logsCollection = mongoDatabase.GetCollection<Log>(
                MongoDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<Log>> GetAsync() =>
       await _logsCollection.Find(_ => true).ToListAsync();

        public async Task<Log?> GetAsync(string id) =>
            await _logsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Log newLog) =>
            await _logsCollection.InsertOneAsync(newLog);

        public async Task UpdateAsync(string id, Log updatedLog) =>
            await _logsCollection.ReplaceOneAsync(x => x.Id == id, updatedLog);

        public async Task RemoveAsync(string id) =>
            await _logsCollection.DeleteOneAsync(x => x.Id == id);
    }
}

using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace OTP_WEBSERVER.Models
{
    public class DBContext
    {
        private readonly IMongoDatabase _database = null;
        public DBContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<User> Users { get { return _database.GetCollection<User>("User"); } }
        public IMongoCollection<Application> Applications { get { return _database.GetCollection<Application>("Application"); } }        
    }
}
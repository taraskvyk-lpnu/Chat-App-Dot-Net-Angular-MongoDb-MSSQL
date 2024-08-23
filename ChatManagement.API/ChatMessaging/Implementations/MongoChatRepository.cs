using ChatMessaging.Contracts;
using ChatMessaging.Models;
using MongoDB.Driver;

namespace ChatMessaging.Implementations;

public class MongoChatRepository : IChatRepository
{
    private readonly IMongoDatabase _database;

    public MongoChatRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
        _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
    }
    
    public async Task<List<Message>> GetMessagesAsync(Guid chatId)
    {
        var collection = GetMessageCollection(chatId);
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task SaveMessageAsync(Guid chatId, Message message)
    {
        try
        {
            var collection = GetMessageCollection(chatId);
            await collection.InsertOneAsync(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private IMongoCollection<Message> GetMessageCollection(Guid chatId)
    {
        return _database.GetCollection<Message>($"Messages_{chatId}");
    }
}
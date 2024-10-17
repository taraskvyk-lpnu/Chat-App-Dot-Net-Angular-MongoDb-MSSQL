using ChatMessaging.Contracts;
using ChatMessaging.Models;
using MongoDB.Driver;

namespace ChatMessaging.DataAccess.Repository.Implementations;

public class MongoMessageRepository : IMessageRepository
{
    private readonly ChatDbContext _chatDbContext;

    public MongoMessageRepository(ChatDbContext chatDbContext)
    {
        _chatDbContext = chatDbContext ?? throw new ArgumentNullException(nameof(chatDbContext));
    }
    
    public async Task<List<Message>> GetMessagesAsync(Guid chatId)
    {
        var messageCollection = _chatDbContext.GetMessageCollection(chatId);
        return await messageCollection.Find(_ => true).ToListAsync();
    }
    
    public async Task<List<Message>> GetMessagesByUserAsync(Guid chatId, Guid userId)
    {
        var messageCollection = _chatDbContext.GetMessageCollection(chatId);
        
        if (messageCollection == null)
        {
            throw new Exception($"{nameof(Message)} collection not found.");
        }
        
        var chatIdFilter = Builders<Message>.Filter.Eq(m => m.ChatId, chatId);
        var senderIdFilter = Builders<Message>.Filter.Eq(m => m.SenderId, userId);
        var combinedFilter = Builders<Message>.Filter.And(chatIdFilter, senderIdFilter);
        
        return await messageCollection.Find(combinedFilter).ToListAsync();
    }

    
    public async Task AddMessageAsync(Guid chatId, Message message)
    {
        var messageCollection = _chatDbContext.GetMessageCollection(chatId);
        
        if (messageCollection == null)
        {
            throw new Exception("Message collection not found.");
        }
        
        message.CreatedAt = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        await messageCollection.InsertOneAsync(message);
    }
    
    public async Task UpdateMessageAsync(Guid chatId, Guid userId, Message message)
    {
        var messageCollection = _chatDbContext.GetMessageCollection(chatId);
        
        if (messageCollection == null)
        {
            throw new Exception("Message collection not found.");
        }
        
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ChatId, chatId),
            Builders<Message>.Filter.Eq(m => m.Id, message.Id),
            Builders<Message>.Filter.Eq(m => m.SenderId, userId)
        );
        
        var update = Builders<Message>.Update
            .Set(m => m.Text, message.Text)
            /*.Set(m => m.SentAt, message.SentAt)*/;
        
        var updateResult = await messageCollection.UpdateOneAsync(filter, update)!;
        
        if (updateResult.MatchedCount == 0)
        {
            throw new Exception("Message not found or user does not have permission to update this message.");
        }
    }
    
    public async Task DeleteMessageAsync(Guid chatId, Guid messageId, Guid userId)
    {
        var messageCollection = _chatDbContext.GetMessageCollection(chatId);
        
        if (messageCollection == null)
        {
            throw new Exception("Message collection not found.");
        }
        
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.ChatId, chatId),
            Builders<Message>.Filter.Eq(m => m.Id, messageId),
            Builders<Message>.Filter.Eq(m => m.SenderId, userId)
        );

        var deleteResult = await messageCollection.DeleteOneAsync(filter);

        if (deleteResult.DeletedCount == 0)
        {
            throw new AccessViolationException("Message not found or user does not have permission to delete this message.");
        }
    }
}
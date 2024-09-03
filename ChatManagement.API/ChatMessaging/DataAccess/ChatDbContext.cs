﻿using ChatMessaging.Models;
using MongoDB.Driver;

namespace ChatMessaging.DataAccess;

public class ChatDbContext
{
    private readonly IMongoDatabase _database;
    
    public ChatDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
        _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
    }
}
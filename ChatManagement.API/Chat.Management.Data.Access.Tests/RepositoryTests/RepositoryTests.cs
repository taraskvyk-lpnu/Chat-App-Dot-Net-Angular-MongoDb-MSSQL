using ChatManagement.DataAccess;
using ChatManagement.Infrastructure.CustomException;
using ChatManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using ChatDomain = ChatManagement.Domain.Models.Chat;

namespace Chat.Management.Data.Access.Tests.RepositoryTests;

public class RepositoryTests : IDisposable
{
    private readonly ChatManagementDbContext _context;
    private readonly Repository<ChatDomain> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ChatManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ChatManagementDbContext(options);
        _repository = new Repository<ChatDomain>(_context);

        // Seed data
        _context.Chats.Add(new ChatDomain
        {
            Title = "Chat",
            CreatorId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            UserIds = [new Guid()]
        });
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
    {
        var chat = await _repository.GetByIdAsync(_context.Chats.First().Id);
        Assert.NotNull(chat);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        var chats = await _repository.GetAllAsync();
        Assert.Single(chats); // Should pass now with a single entity in the context
    }

    [Fact]
    public async Task AddAsync_AddsEntity()
    {
        var newChat = new ChatDomain { Id = Guid.NewGuid() };
        await _repository.AddAsync(newChat);

        var chatInDb = await _context.Chats.FindAsync(newChat.Id);
        Assert.NotNull(chatInDb);
    }

    [Fact]
    public async Task RemoveByIdAsync_RemovesEntity_WhenEntityExists()
    {
        var existingChat = _context.Chats.First();
        await _repository.RemoveByIdAsync(existingChat.Id);

        var chatInDb = await _context.Chats.FindAsync(existingChat.Id);
        Assert.Null(chatInDb);
    }

    [Fact]
    public async Task RemoveByIdAsync_ThrowsNotFoundException_WhenEntityDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        await Assert.ThrowsAsync<NotFoundException>(() => _repository.RemoveByIdAsync(nonExistentId));
    }
}
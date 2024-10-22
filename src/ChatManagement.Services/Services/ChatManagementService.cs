using AutoMapper;
using ChatManagement.Domain;
using ChatManagement.Domain.Models.ChatRequests;
using ChatManagement.Domain.Models.Dtos;
using ChatManagement.Domain.Services;
using ChatManagement.Infrastructure.CustomException;

namespace ChatManagement.Services.Services;

public class ChatManagementService : IChatManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ChatManagementService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<ChatDto> AddChatAsync(AddChatRequest addChatRequest)
    {
        var chatDto = _mapper.Map<ChatDto>(addChatRequest);
        
        var chat = await _unitOfWork.Chat.AddChatAsync(chatDto, addChatRequest.CreatorId);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<ChatDto>(chat);
    }

    public async Task<ChatDto> UpdateChatAsync(UpdateChatRequest updateChatRequest)
    {
        var chatDto = _mapper.Map<ChatDto>(updateChatRequest);
        
        var chat = await _unitOfWork.Chat.UpdateChatAsync(chatDto, updateChatRequest.UserId);
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<ChatDto>(chat);
    }

    public async Task RemoveChatAsync(RemoveChatRequest deleteChatRequest)
    {
        await _unitOfWork.Chat.RemoveChatAsync(deleteChatRequest.ChatId, deleteChatRequest.UserId);
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<ChatDto>> GetAllChatsAsync()
    {
        var chats = await _unitOfWork.Chat.GetAllAsync();
        
        if (chats == null)
        {
            throw new NotFoundException($"Chats do not exist");
        }
        
        return _mapper.Map<IEnumerable<ChatDto>>(chats);
    }

    public async Task<ChatDto> GetChatByIdAsync(Guid chatId)
    {
        var chat = await _unitOfWork.Chat.GetByIdAsync(chatId);
        
        if (chat == null)
        {
            throw new NotFoundException($"Chat with id '{chatId}' does not exist");
        }
        
        return _mapper.Map<ChatDto>(chat);
    }
    
    public async Task<IEnumerable<ChatDto>> GetChatsByUserIdAsync(Guid userId)
    {
        var chats = await _unitOfWork.Chat.GetChatsByUserIdAsync(userId);
        
        if (chats == null)
        {
            throw new NotFoundException($"Chats for user with id '{userId}' does not exist");
        }
        
        return _mapper.Map<IEnumerable<ChatDto>>(chats);
    }
    
    public async Task<ChatDto> AttachUserToChatAsync(AttachUserRequest addUserToChatRequest)
    {
        var chat =  await _unitOfWork.Chat.AttachUserToChatAsync(addUserToChatRequest.ChatId, addUserToChatRequest.UserToAddId);
        await _unitOfWork.CommitAsync();
        
        return _mapper.Map<ChatDto>(chat);
    }
    public async Task<ChatDto> DetachUserFromChatAsync(DetachUserRequest detachUserRequest)
    {
        var chat = await _unitOfWork.Chat.DetachUserFromChatAsync(detachUserRequest.ChatId, detachUserRequest.UserToDetachId);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<ChatDto>(chat);
    }
}
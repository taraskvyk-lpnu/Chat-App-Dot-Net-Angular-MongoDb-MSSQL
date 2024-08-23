using ChatMessaging.Contracts;
using ChatMessaging.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatMessaging
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository _chatRepository;

        public ChatHub(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task JoinChat(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            await LoadMessages(chatId);
        }

        public async Task SendMessage(Guid chatId, Message message)
        {
            await _chatRepository.SaveMessageAsync(chatId, message);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatId, message);
        }

        public async Task LoadMessages(Guid chatId)
        {
            var messages = await _chatRepository.GetMessagesAsync(chatId);
            await Clients.Caller.SendAsync("ReceiveMessages", chatId, messages);
        }

        public async Task LeaveChat(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
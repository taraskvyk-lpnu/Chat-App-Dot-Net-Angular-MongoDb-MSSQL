using ChatMessaging.Contracts;
using ChatMessaging.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatMessaging
{
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
        }

        public async Task SendMessage(Message message, Guid chatId)
        {
            await _chatRepository.SaveMessageAsync(chatId, message);
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message.UserName, message);
        }

        public async Task LoadMessages(Guid chatId)
        {
            var messages = await _chatRepository.GetMessagesAsync(chatId);
            await Clients.Caller.SendAsync("ReceiveMessages", messages);
        }

        public async Task LeaveChat(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
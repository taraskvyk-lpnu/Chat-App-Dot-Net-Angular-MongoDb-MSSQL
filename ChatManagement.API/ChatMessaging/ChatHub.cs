using ChatMessaging.Contracts;
using ChatMessaging.Models;
using ChatMessaging.Models.MessageRequests;
using ChatMessaging.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatMessaging
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task JoinChat(Guid chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            await LoadMessagesAsync(chatId);
        }

        public async Task SendMessage(AddMessageRequest request)
        {
            await _messageService.AddMessageAsync(request);
            await Clients.Group(request.ChatId.ToString()).SendAsync("ReceiveMessage", request.ChatId, request.Message);
        }

        private async Task LoadMessagesAsync(Guid chatId)
        {
            var messages = await _messageService.GetMessagesAsync(chatId);
            await Clients.Caller.SendAsync("ReceiveMessages", chatId, messages);
        }

        public async Task LeaveChat(Guid chatId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
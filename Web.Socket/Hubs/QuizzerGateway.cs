using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Web.Socket.Services;

namespace Web.Socket.Hubs
{
    public class QuizzerGateway : Hub
    {
        private readonly QuizService _service;

        public QuizzerGateway(QuizService service)
        {
            _service = service;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var queryRoomId = httpContext.Request.Query["roomId"];

            if (!ulong.TryParse(queryRoomId, out ulong roomId))
                throw new ArgumentException("Room Id should be ulong type");

            var exist = await _service.QuizExist(roomId);

            if (exist == null)
            {
                await Clients.Caller.SendAsync("Error", "Room does not exist");
                Context.Abort();
            }

            await _service.JoinGame(roomId, Context.User?.Identity?.Name ?? Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        [HubMethodName("StartGame")]
        public async Task StartGame(ulong id)
        {
            await _service.StartQuiz(id);
        }

        [HubMethodName("NextQuestion")]
        public async Task NextQuestion(ulong id)
        {
            await _service.NextQuestion(id);
        }

        public Task SendMessage(string user, string message)
        {
            return Clients.All.SendAsync("ReceivedMessage", user, message);
        }

    }
}

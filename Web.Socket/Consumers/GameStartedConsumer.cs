using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Quizzer.Domain.Events;
using Web.Socket.Hubs;

namespace Web.Socket.Consumers
{
    public class GameStartedConsumer : IConsumer<GameStartedEvent>
    {
        private readonly IHubContext<QuizzerGateway> _gateway;

        public GameStartedConsumer(IHubContext<QuizzerGateway> gateway)
        {
            _gateway = gateway;
        }

        public Task Consume(ConsumeContext<GameStartedEvent> context)
        {
            _gateway.Clients.All.SendAsync("GameStarted", context.Message);
            return Task.CompletedTask;
        }
    }
}

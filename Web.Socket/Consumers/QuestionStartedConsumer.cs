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
    public class QuestionStartedConsumer : IConsumer<QuestionStartedEvent>
    {
        private readonly IHubContext<QuizzerGateway> _gateway;

        public QuestionStartedConsumer(IHubContext<QuizzerGateway> gateway)
        {
            _gateway = gateway;
        }

        public Task Consume(ConsumeContext<QuestionStartedEvent> context)
        {
            _gateway.Clients.All.SendAsync("QuestionReceived", context.Message);
            return Task.CompletedTask;
        }
    }
}

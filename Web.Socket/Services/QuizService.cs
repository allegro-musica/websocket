using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Quiz.API;

namespace Web.Socket.Services
{
    public class QuizService
    {
        private readonly IConfiguration _config;

        public QuizService(HttpClient http, IConfiguration config)
        {
            _config = config;
        }

        public Task<QuizExistResponse> QuizExist(ulong id)
        {
            return GrpcCallerService.CallService(_config.GetValue<string>("QuizGrpcApi"), async channel =>
            {
                var client = new Quizer.QuizerClient(channel);

                return await client.QuizExistAsync(new QuizExistRequest()
                {
                    Id = id
                });
            });
        }

        public Task StartQuiz(ulong id)
        {
            return GrpcCallerService.CallService(_config.GetValue<string>("QuizGrpcApi"), async channel =>
            {
                var client = new Quizer.QuizerClient(channel);

                return await client.StartGameAsync(new QuizStartRequest()
                {
                    Id = id
                });
            });
        }

        public Task JoinGame(ulong id, string user)
        {
            return GrpcCallerService.CallService(_config.GetValue<string>("QuizGrpcApi"), async channel =>
            {
                var client = new Quizer.QuizerClient(channel);

                return await client.JoinGameAsync(new JoinGameRequest()
                {
                    Id = id,
                    User = user
                });
            });
        }

        public Task NextQuestion(ulong id)
        {
            return GrpcCallerService.CallService(_config.GetValue<string>("QuizGrpcApi"), async channel =>
            {
                var client = new Quizer.QuizerClient(channel);

                await client.NextQuestionAsync(new NextQuestionRequest()
                {
                    Id = id
                });
            });
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Exercises.Core.Hubs
{
    public class GameHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;

        public GameHub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SendMessage(string user, string message)
        {
            var clients = Clients.All;
            await clients.SendAsync("ReceiveMessage", user, message);
        }

        public async Task ReceiveHello()
        {
            var clients = Clients.Caller;
            await clients.SendAsync("SayHello", "Hello from signalR");
        }

        public async Task GoToNextExercise(Guid examId)
        {
            var service = (IExamService) _serviceProvider.GetService(typeof(IExamService));
            await service.GoToNextExercise(examId, CancellationToken.None);
        }

        public async Task EndGame(Guid examId)
        {
            var service = (IExamService) _serviceProvider.GetService(typeof(IExamService));
            await service.EndGame(examId, CancellationToken.None);
        }
    }
}
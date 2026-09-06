using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exercises.Common.Abstractions;
using Exercises.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Exercises.Core.Hubs
{
    /// <summary>
    /// Round control for a running game.
    /// </summary>
    /// <remarks>
    /// Every method here mutates shared game state for every participant, so the hub is
    /// authenticated and each call is checked against the exam it names. Without that,
    /// any connected client — including an anonymous one, since the hub used to require
    /// no credentials at all — could invoke <see cref="EndGame"/> with someone else's
    /// exam id and terminate a class's exam, or skip everyone to the next exercise.
    /// </remarks>
    [Authorize(Constants.POLICY_GAME_HUB)]
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
            var service = await AuthorizeGameControl(examId, CancellationToken.None);
            await service.GoToNextExercise(examId, CancellationToken.None);
        }

        public async Task EndGame(Guid examId)
        {
            var service = await AuthorizeGameControl(examId, CancellationToken.None);
            await service.EndGame(examId, CancellationToken.None);
        }

        /// <summary>
        /// Resolves the exam service, having established that the caller is allowed to
        /// drive this particular game: either the timer worker, or the teacher who owns
        /// the exam.
        /// </summary>
        private async Task<IExamService> AuthorizeGameControl(Guid examId, CancellationToken token)
        {
            var service = (IExamService) _serviceProvider.GetService(typeof(IExamService));

            // The worker drives progression for every game and owns none of them.
            if (Context.User?.IsInRole(Constants.ROLE_SERVICE) == true)
            {
                return service;
            }

            var ownerId = await service.GetOwnerId(examId, token);
            if (ownerId == null)
            {
                throw new HubException("Game not found.");
            }

            if (ownerId != GetCallerId())
            {
                throw new HubException("You are not allowed to control this game.");
            }

            return service;
        }

        private Guid GetCallerId()
        {
            var id = Context.User?.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            return Guid.TryParse(id, out var parsed) ? parsed : Guid.Empty;
        }
    }
}

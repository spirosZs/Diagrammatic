using System;
using System.Threading.Tasks;
using Exercises.Core.Hubs;
using Exercises.Data.Types;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Exercises.Core.Helpers
{
    public static class GameExtensions
    {
        public static async Task NotifyAllEvent(this IHubContext<GameHub> hubContext, GameEventType eventType,
            object parameters)
        {
            var clients = hubContext.Clients.All;
            var json = JsonConvert.SerializeObject(parameters);
            await clients.SendAsync("Notify", Enum.GetName(typeof(GameEventType), eventType), json);
        }
    }
}
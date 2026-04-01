using System.Collections.Concurrent;

namespace WeatherBot.Services.LineMessaging.UserPreferences
{
    public class InMemoryLocationChangeStateStore : ILocationChangeStateStore
    {
        private readonly ConcurrentDictionary<string, byte> _awaitingUsers = new();

        public bool IsAwaitingLocationInput(string userId)
            => !string.IsNullOrWhiteSpace(userId) && _awaitingUsers.ContainsKey(userId);

        public void SetAwaitingLocationInput(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            _awaitingUsers[userId] = 0;
        }

        public void ClearAwaitingLocationInput(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            _awaitingUsers.TryRemove(userId, out _);
        }
    }
}
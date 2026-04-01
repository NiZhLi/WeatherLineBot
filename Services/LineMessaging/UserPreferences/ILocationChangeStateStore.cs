namespace WeatherBot.Services.LineMessaging.UserPreferences
{
    public interface ILocationChangeStateStore
    {
        bool IsAwaitingLocationInput(string userId);
        void SetAwaitingLocationInput(string userId);
        void ClearAwaitingLocationInput(string userId);
    }
}
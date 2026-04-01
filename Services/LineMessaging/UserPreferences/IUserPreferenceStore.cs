using System.Threading;
using System.Threading.Tasks;
using WeatherBot.Models;

namespace WeatherBot.Services.LineMessaging.UserPreferences
{
    public interface IUserPreferenceStore
    {
        Task<UserPreference> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task SetLocationAsync(string userId, string defaultLocation, CancellationToken cancellationToken = default);
    }
}

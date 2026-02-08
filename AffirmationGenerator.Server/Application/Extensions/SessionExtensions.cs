using System.Text.Json;

namespace AffirmationGenerator.Server.Application.Extensions;

public static class SessionExtensions
{
    extension(ISession session)
    {
        public void Set<T>(string key, T value) => session.SetString(key, JsonSerializer.Serialize(value));

        public T? Get<T>(string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}

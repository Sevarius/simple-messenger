using Microsoft.AspNetCore.Builder;
using SignalRApi.Hubs;

namespace SignalRApi.Configuration;

public static class SignalRConfiguration
{
    public static WebApplication MapSignalRHubs(this WebApplication app)
    {
        app.MapHub<MessagesHub>("hubs/messages");
        app.MapHub<UserStatusHub>("hubs/user-statuses");

        return app;
    }
}

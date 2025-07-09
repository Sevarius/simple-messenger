using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Threading.Tasks;
using SignalRSwaggerGen.Attributes;

namespace WebApi.Hubs;

[SignalRHub]
public class CommandHub : Hub
{
    private static readonly ILogger Logger = Log.ForContext<CommandHub>();

    public async Task SendCommand(string message)
    {
        Logger.Information("Received command: {Message} from connection {ConnectionId}", message, Context.ConnectionId);
        
        // Echo the command back to all connected clients
        await Clients.All.SendAsync("CommandReceived", message);
    }
} 
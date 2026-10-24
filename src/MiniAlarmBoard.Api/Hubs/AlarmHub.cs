using Microsoft.AspNetCore.SignalR;

namespace MiniAlarmBoard.Api.Hubs;

public class AlarmHub : Hub<IAlarmClient>
{
    public async Task WatchArea(string area)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, area);
    }
}

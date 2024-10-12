using Microsoft.AspNetCore.SignalR;

namespace Restauran_API.SignalR
{
    public class MenuItemHub : Hub
    {
        public async Task NotifyNewMenuItem(string updateItem)
        {
            await Clients.All.SendAsync("ReceiveMenuUpdate", updateItem);
        }
    }
}
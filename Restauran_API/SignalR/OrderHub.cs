using Microsoft.AspNetCore.SignalR;

namespace Restauran_API.SignalR
{
    public class OrderHub : Hub
    {
        public async Task NotifyNewOrder(string updateOrder)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", updateOrder);
        }
    }
}
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WildCompile.WebSockets
{
    public class IOHandler : WebSocketHandler
    {
        public IOHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            var user = WebSocketConnectionManager.GetSocketBySocket(socket);
            await SendMessageAsync(socket, $"{{cmd:\"register\", data:\"{user.Id}\"}}");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var userSocket = WebSocketConnectionManager.GetSocketBySocket(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var user = WebSocketConnectionManager.GetSocketBySocket(socket);
            var sendMessage = $"{user.Username ?? user.Id} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            await SendMessageToAllAsync(sendMessage);
        }
    }
}

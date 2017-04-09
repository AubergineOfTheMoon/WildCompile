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

            var socketId = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{socketId} is now connected");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var userSocket = WebSocketConnectionManager.GetSocketBySocket(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            if (message.StartsWith("register:"))
            {
                var username = message.Substring(9);
                if (username.Length <= 0 || username.Length > 10 || !username.All(char.IsLetterOrDigit))
                {
                    await SendMessageAsync(socket, "Username must be between 0 and 10 characters and only contain letters and numbers." + username);
                    return;
                }
                userSocket.Username = username;
                await SendMessageAsync(socket, "Registered!");
                return;
            }
            var user = WebSocketConnectionManager.GetSocketBySocket(socket);
            var sendMessage = $"{user.Username ?? user.Id} said: {Encoding.UTF8.GetString(buffer, 0, result.Count)}";
            await SendMessageToAllAsync(sendMessage);
        }
    }
}

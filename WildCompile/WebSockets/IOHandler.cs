using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using WildCompile.Models;

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
            await SendMessageAsync(socket, JsonConvert.SerializeObject(new { cmd = "register", data = user.Id}));
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            SocketMessage content = null;
            try
            {
                content = JsonConvert.DeserializeObject<Models.SocketMessage>(message);
            }
            catch (System.Exception)
            {
                return;
            }
            switch (content.cmd.ToUpper())
            {
                case "INPUT":
                    //User
                    var user = WebSocketConnectionManager.GetSocketBySocket(socket);
                    var inputMessage = new SocketMessage()
                    {
                        cmd = "input",
                        data = content.data,
                        id = user.Id
                    };
                    var inputMessageText = JsonConvert.SerializeObject(inputMessage);
                    await SendMessageAsync(user.ServerId, inputMessageText);
                    break;
                case "OUTPUT":
                    //Server
                    //var msg = new SocketMessage();
                    //msg.cmd = "clear";
                    //var msgText = JsonConvert.SerializeObject(msg);
                    //await SendMessageAsync(content.id, msgText);

                    //Sets the server id of the user
                    WebSocketConnectionManager.GetSocketById(content.id).ServerId = WebSocketConnectionManager.GetId(socket);

                    var msg = new SocketMessage();
                    msg.cmd = "print";
                    msg.id = content.id;
                    msg.data = content.data;
                    var msgText = JsonConvert.SerializeObject(msg);
#if DEBUG
                    if (content.id.ToUpper() != "TESTING")
                    {
                        await SendMessageAsync(content.id, msgText);
                    }
#else
                    await SendMessageAsync(content.id, msgText);
#endif
                    break;
                case "ASKINPUT":
                    //Sets the server id of the user
                    WebSocketConnectionManager.GetSocketById(content.id).ServerId = WebSocketConnectionManager.GetId(socket);

                    var askInputMsg = new SocketMessage()
                    {
                        cmd = "input"
                    };
                    var askInputMsgText = JsonConvert.SerializeObject(askInputMsg);
#if DEBUG
                    if (content.id.ToUpper() != "TESTING")
                    {
                        await SendMessageAsync(content.id, askInputMsgText);
                    }
#else
                    await SendMessageAsync(content.id, askInputMsgText);
#endif
                    break;
                default:
                    break;
            }
        }
    }
}

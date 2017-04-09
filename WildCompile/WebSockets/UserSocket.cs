using System.Net.WebSockets;

namespace WildCompile.WebSockets
{
    public class UserSocket
    {
        public WebSocket Socket { get; private set; }
        public string Id { get; private set; }
        public string Username { get; set; }
        public string Key { get; set; }
        public string ServerId { get; set; }

        public UserSocket(WebSocket Socket, string Id)
        {
            this.Socket = Socket;
            this.Id = Id;
        }
    }
}

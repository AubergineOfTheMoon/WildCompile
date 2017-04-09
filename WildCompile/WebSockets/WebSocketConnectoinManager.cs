using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WildCompile.WebSockets
{
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, UserSocket> _sockets = new ConcurrentDictionary<string, UserSocket>();

        public UserSocket GetSocketById(string id)
        {
            return _sockets[id];
        }

        public UserSocket GetSocketBySocket(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value.Socket == socket).Value;
        }

        public ConcurrentDictionary<string, UserSocket> GetAll()
        {
            return _sockets;
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.Values.FirstOrDefault(p => p.Socket == socket).Id;
            //return _sockets.FirstOrDefault(p => p.Value.Socket == socket).Key;
        }
        public void AddSocket(WebSocket socket)
        {
            string id = CreateConnectionId();
            var userSocket = new UserSocket(socket, id);
            _sockets.TryAdd(id, userSocket);
        }

        public async Task RemoveSocket(string id)
        {
            //WebSocket socket;
            UserSocket socket;
            _sockets.TryRemove(id, out socket);

            await socket.Socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketManager",
                                    cancellationToken: CancellationToken.None);
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
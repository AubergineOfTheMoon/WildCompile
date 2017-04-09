using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WildCompile
{
    public static class ExtensionMethods
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                                  PathString path,
                                                                  WebSockets.WebSocketHandler handler)
        {
            return app.Map(path, (_app) => _app.UseMiddleware<WebSockets.WebSocketManagerMiddleware>(handler));
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSockets.WebSocketConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSockets.WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}

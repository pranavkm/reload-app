using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;

namespace a1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            _ = Task.Run(async () =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_AUTO_RELOAD_WS_ENDPOINT");
                var client = new ClientWebSocket();
                await client.ConnectAsync(new Uri(env), default);
                var buffer = new byte[4 * 1024 * 1024];
                while (client.State == WebSocketState.Open)
                {
                    var receive = await client.ReceiveAsync(buffer, default);
                    if (receive.CloseStatus is not null)
                    {
                        System.Console.WriteLine(receive.CloseStatus);
                        break;
                    }

                    System.Console.WriteLine("Received message");

                    UpdateDelta update;
                    try
                    {
                        update = JsonSerializer.Deserialize<UpdateDelta>(buffer.AsSpan(0, receive.Count), new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        // Ignore these. It's probably a message for the browser.
                        continue;
                    }

                    if (update.Type != "UpdateCompilation")
                    {
                        continue;
                    }

                    try
                    {
                        typeof(Program).Assembly.ApplyUpdate(update.MetaBytes, update.IlBytes, update.PdbBytes);
                        Console.Write("Applied diff");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }

                Console.WriteLine("Exited update loop");
            });


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private readonly struct UpdateDelta
        {
            public string Type  { get; init; }

            public string ModulePath { get; init;  }
            public byte[] MetaBytes { get; init; }
            public byte[] IlBytes { get; init; }
            public byte[] PdbBytes { get; init; }
        }
    }
}

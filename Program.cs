using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace HttpListenerTest
{
    internal class Program
    {
        private static readonly string JsonFilePath = "data.json";

        static async Task Main(string[] args)
        {
            // 소켓 데이터 수신 시작
            Task.Run(ReceiveSocketData);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:13000/");
            listener.Start();
            Console.WriteLine("HTTP Server started. Waiting for connections...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                Task.Run(() => HandleRequest(context));
            }
        }

        private static async Task HandleRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            try
            {
                string requestPath = request.Url.AbsolutePath;

                if (requestPath == "/chartData")
                {
                    string chartData = GetChartData();
                    byte[] buffer = Encoding.UTF8.GetBytes(chartData);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    byte[] buffer = Encoding.UTF8.GetBytes("Not Found");
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                byte[] buffer = Encoding.UTF8.GetBytes("Internal server error.");
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

        private static string GetChartData()
        {
            try
            {
                string jsonData = File.ReadAllText(JsonFilePath);
                return jsonData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while reading JSON file: {ex.Message}");
                return "[]";
            }
        }

        private static async Task ReceiveSocketData()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // 소켓 바인딩 및 수신 대기 로직 추가 필요

                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        File.AppendAllText(JsonFilePath, data);
                    }
                }
            }
        }
    }
}
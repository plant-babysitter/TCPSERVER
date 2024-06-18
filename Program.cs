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
        } 미구현

        private static async Task ReceiveSocketData()
        {
            static async Task Main(string[] args)
        {
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
                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string received = await reader.ReadToEndAsync();
                    int jsonStartIndex = received.IndexOf("{");
                    if (jsonStartIndex != -1)
                    {
                        string jsonData = received.Substring(jsonStartIndex);
                        try
                        {
                            var json = JsonDocument.Parse(jsonData);
                            Console.WriteLine($"Parsed JSON: {json.RootElement.ToString()}");

                            // 응답 데이터 설정
                            string responseString = "JSON received and parsed successfully.";
                            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                            response.ContentLength64 = buffer.Length;
                            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                            response.StatusCode = (int)HttpStatusCode.BadRequest;
                            byte[] buffer = Encoding.UTF8.GetBytes("Invalid JSON format.");
                            response.ContentLength64 = buffer.Length;
                            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                        }
                    }
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
    }
        }
    }
}
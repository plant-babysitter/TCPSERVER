using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpListenerTest
{
    internal class Program
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

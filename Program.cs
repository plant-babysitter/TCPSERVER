using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;


namespace TcpListenerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {

            TcpListener server = null;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1"); int port = 13000;
            try
            {
                server = new TcpListener(localAddr, port);
                server.Start();

                while (true)
                {


                    Console.WriteLine("waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    
                    NetworkStream stream = client.GetStream();
                    byte[] readBuffer = new byte[1024];

                    try
                    {

                        int bytesRead;
                        //read bufferSize
                        while ((bytesRead = stream.Read(readBuffer, 0, readBuffer.Length)) != 0)
                        {
                            

                            string received = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                            Console.WriteLine("Received: {0}", received);

                           


                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception:{0}", ex.Message);
                    }
                    finally
                    {
                        stream.Close();
                        client.Close();


                    }
                }
            }
            catch (SocketException e)
            {

                Console.WriteLine("\r\n서버가 종료됩니다.");


            }
            finally
            {
                server.Stop();
                Console.WriteLine("Server stopped");
            }
        }
    }
}
    




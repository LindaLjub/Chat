using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ConsoleAppClient
{

    class Program
    {
        public static string HasseIp = "172.16.117.80";
        public static string OsamaIp = "172.16.118.44";
        public static string StefanIp = "172.16.117.92";

        // Main method 
        static void Main(string[] args)
        {
            // Creating and initializing thread 
            Thread ListenThr = new Thread(listenThread);
            Thread MessageThr = new Thread(MessageThread);

            ListenThr.Start();
            MessageThr.Start();
        }

        static void listenThread()
        {
            IPAddress localAddr = IPAddress.Parse("172.16.117.71");
            TcpListener server = new TcpListener(localAddr, 8080);
            TcpClient client = default(TcpClient);
            try
            {
                server.Start();
                Console.WriteLine("Chat started...");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            while (true)
            {
                client = server.AcceptTcpClient();
                string ip = client.Client.RemoteEndPoint.ToString();

                byte[] recievedBuffer = new byte[100];
                NetworkStream stream = client.GetStream();

                try
                {
                    stream.Read(recievedBuffer, 0, recievedBuffer.Length);
                }
                catch (Exception)
                {
                    Console.WriteLine("Stream could not be read..");
                }

                StringBuilder msg = new StringBuilder();

                foreach (byte b in recievedBuffer)
                {
                    if (b.Equals(0))
                    {
                        break;
                    }
                    else
                    {
                        msg.Append(Convert.ToChar(b).ToString());
                    }
                }

                if (msg.ToString() != "")
                {
                    int byteCount = Encoding.ASCII.GetByteCount("Message delivered!");
                    byte[] sendData = new byte[byteCount];

                    sendData = Encoding.ASCII.GetBytes("Message delivered!");
                    stream.Write(sendData, 0, sendData.Length);
                    stream.Close();
                    client.Close();
                }

                // Blue color from other users
                Console.ForegroundColor = ConsoleColor.Blue;

                string subname = ip.Substring(0, 13);
                string chatname = "";

                if (subname == HasseIp)
                {
                    chatname = "Hasse";
                    
                }
                else if(subname == StefanIp)
                {
                    chatname = "Stefan";
                }
                else if (subname == OsamaIp)
                {
                    chatname = "Osama";
                }
                else
                {
                    chatname = subname;
                }

                Console.WriteLine(msg.ToString() + " from " + chatname);

                // White color when I write
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void MessageThread()
        {
            string serverIP = HasseIp;
            int port = 8080;

            while (true)
            {
                string message = Console.ReadLine();

                try
                {
                    // SEND DATA
                    TcpClient clienta = new TcpClient(serverIP, port);
                    int byteCount = Encoding.ASCII.GetByteCount(message);

                    byte[] sendDataa = new byte[byteCount];

                    sendDataa = Encoding.ASCII.GetBytes(message);
                    NetworkStream stream = clienta.GetStream();
                    stream.Write(sendDataa, 0, sendDataa.Length);

                    // RESPONSE
                    // Buffer to store the response bytes.
                    sendDataa = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;

                    // Read the first batch of the TcpServer response bytes.
                    Int32 bytes = stream.Read(sendDataa, 0, sendDataa.Length);
                    responseData = System.Text.Encoding.ASCII.GetString(sendDataa, 0, bytes);
                    Console.WriteLine(responseData);

                    stream.Close();
                    clienta.Close();

                }
                catch(Exception)
                {
                    Console.WriteLine("Message could not be sent..");
                }
            }
        }
    }
}

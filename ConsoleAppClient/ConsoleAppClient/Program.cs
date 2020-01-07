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

        // Main method 
        static void Main(string[] args)
        {
           
            // Creating and initializing thread 
            Thread ListenThr = new Thread(listenThread);
            Thread MessageThr = new Thread(MessageThread);

            ListenThr.Start();
            MessageThr.Start();
            // Console.WriteLine("Main Thread Ends!!");
  
        }

        // Static method 
        static void listenThread()
        {
            //for (int c = 0; c <= 3; c++)
            //{

            //    Console.WriteLine("Listen is in progress!!");
            //    Thread.Sleep(1000);
            //}
            //Console.WriteLine("Listen ends!!");

            IPAddress localAddr = IPAddress.Parse("192.168.56.1");
            TcpListener server = new TcpListener(localAddr, 8080);
            TcpClient client = default(TcpClient);
            try
            {
                server.Start();
                Console.WriteLine("Server started...");


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

                stream.Read(recievedBuffer, 0, recievedBuffer.Length);

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
                }

                Console.WriteLine(msg.ToString() + " " + ip);
            }
        }

        static void MessageThread()
        {
            //for (int c = 0; c <= 3; c++)
            //{

            //    Console.WriteLine("Message is in progress!!");
            //    Thread.Sleep(1000);
            //}
            //Console.WriteLine("Message ends!!");

            string serverIP = "192.168.56.1";
            int port = 8080;

            while(true)
            {
                string message = Console.ReadLine();

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
           
        }
    }
}

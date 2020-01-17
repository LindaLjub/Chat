using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ConsoleAppServer
{
    class Program
    {
        static void Main(string[] args)
        {

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
    }
}

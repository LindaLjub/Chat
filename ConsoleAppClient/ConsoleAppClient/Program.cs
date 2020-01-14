using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json; // json
using MySql.Data.MySqlClient; // For mySql connection

namespace ConsoleAppClient
{
    class CreateProtocol
    {
        public string date { get; set; }
        public string senderIP { get; set; }
        public string recipientIP { get; set; }
        public string nickname { get; set; }
        public string message { get; set; }

    }
    // Use in server -> DB
    class sendInfo
    {
        public string date { get; set; }
        public string senderIP { get; set; }
        public string recipientIP { get; set; }
        public string nickname { get; set; }
        public string message { get; set; }

    }

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

                byte[] recievedBuffer = new byte[400];
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

                    // stream.Close();
                    // client.Close();
                    stream.Flush();

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

                string message = msg.ToString();
                Console.WriteLine(chatname + " said \n" + message);

                // send to DB
                sendToDB(message);

                // White color when I write
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        static void MessageThread()
        {
            CreateProtocol protocolObject = new CreateProtocol();
            string serverIP = "172.16.117.71";
            int port = 8080;

            while (true)
            {
                string message = Console.ReadLine();

                // Send with protocol
                createProtocol(protocolObject);

                // create json
                protocolObject.recipientIP = serverIP;
                protocolObject.message = message;
                string protocol = JsonConvert.SerializeObject(protocolObject);

                try
                {
                    // SEND DATA
                    TcpClient clienta = new TcpClient(serverIP, port);
                    int byteCount = Encoding.ASCII.GetByteCount(protocol);

                    byte[] sendDataa = new byte[byteCount];

                    sendDataa = Encoding.ASCII.GetBytes(protocol);
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

                    //stream.Close();
                    //clienta.Close();
                    stream.Flush();

                }
                catch(Exception)
                {
                    Console.WriteLine("Message could not be sent..");
                }
            }
        }

        static void createProtocol(CreateProtocol protocolObject)
        {
            // Retrive Date
            DateTime dateTime = new DateTime();
            dateTime = DateTime.Now;
            String S_DateTime = dateTime.ToString();

            // Get IP-ADRESS
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            // create json
            protocolObject.date = S_DateTime;
            protocolObject.senderIP = myIP;
            protocolObject.nickname = "Linda";

        }

        static void sendToDB(string message)
        {
            // parse info
            sendInfo info = JsonConvert.DeserializeObject<sendInfo>(message);

            // connect to DB
            string server = "localhost";
            string userid = "root";
            string password = "Pa55word";
            string database = "kurs1";

            // connection string
            string cs = "server=" + server + ";userid=" + userid + ";password=" + password + ";database=" + database;

            // connection, create and open
            var con = new MySqlConnection(cs);
            con.Open();

            // try connection
            Console.WriteLine($"MySql version: {con.ServerVersion}");

            // SQL query string
            string sql = "INSERT INTO chat_history(date, senderIP, recipientIP, nickname, message) VALUES('" + info.date + "', '" + info.senderIP + "', '" + info.recipientIP + "', '" + info.nickname + "', '" + info.message + "')";

            // command, request with sql query and sql connection. 
            // cmd = response from db
            var cmd = new MySqlCommand(sql, con);

            // execute query
            cmd.BeginExecuteNonQuery();


            //// to se data
            //string sqla = "SELECT *  FROM chat_history";

            //// command, request with sql query and sql connection. 
            //// cmd = response from db
            //var cmda = new MySqlCommand(sqla, con);

            //// execute reader
            //MySqlDataReader reader = cmda.ExecuteReader();

            //// Show data
            //while (reader.Read())
            //{
            //    Console.WriteLine(reader["Id"] + ": " + reader["date"]);
            //}

            //// close the reader
            //reader.Close();

        }


    }
}

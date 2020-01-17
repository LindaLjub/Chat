using System;
using System.Text;
using System.Net;
using System.Threading; // threads
using System.Net.Sockets; // sockets
using Newtonsoft.Json; // json
using MySql.Data.MySqlClient; // For mySql connection, DB

namespace ConsoleAppClient
{
    // Used to create protocol
    class CreateProtocol
    {
        public string date { get; set; }
        public string senderIP { get; set; }
        public string recipientIP { get; set; }
        public string nickname { get; set; }
        public string message { get; set; }

    }

    // send data to database, Code used in server not here.
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
        static void Main(string[] args)
        {
            // Start the connection
            StartConnection();

        }

        // start connection to server
        static void StartConnection()
        {
            // ip to server
            string serverIP = "172.16.117.80";
            int port = 8080;

            try
            {
                // Connect to server
                TcpClient client = new TcpClient(serverIP, port);
                Console.WriteLine("Connected to server");

                // create stream
                NetworkStream stream = client.GetStream();

                // start 2 threads, stream write & stream read
                // Creating and initializing thread 
                Thread ListenThr = new Thread (() => ListenThread(client, stream));
                Thread SendThr = new Thread (() => SendThread(serverIP, client, stream)); 

                // start threads
                ListenThr.Start();
                SendThr.Start();

            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect");
            }
        }

        // listen for incoming messages
        static void ListenThread(TcpClient client, NetworkStream stream)
        {
            while(true)
            {
                // if there is any data in stream
                if (stream.DataAvailable)
                {
                    // used to extract data from stream
                    byte[] recievedBuffer = new byte[400];
                    StringBuilder msg = new StringBuilder();

                    // read from stream
                    try
                    {
                        stream.Read(recievedBuffer, 0, recievedBuffer.Length);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Stream could not be read..");
                    }

                    // for each byte, append to msg
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

                    // makes a beep when retriving a message
                    Console.Beep(); 

                    // output message
                    // Blue color from other users, white when I write
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\n" + msg + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("'linda' says > ");

                    // flush stream when done
                    stream.Flush();     
                }
            }     
        }

        // send messages to server
        static void SendThread(string serverIP, TcpClient client, NetworkStream stream)
        {
            // protocol object, used to send right protocol
            CreateProtocol protocolObject = new CreateProtocol();

            while (true)
            {
                // read from console
                Console.Write("'linda' says > ");
                string message = Console.ReadLine();

                // clear console
                if (message == "clear")
                {
                    Console.Clear();
                }
                else
                {
                    // Add data to protcol object
                    createProtocol(protocolObject);
                    protocolObject.recipientIP = serverIP;
                    protocolObject.message = message;

                    // Create JSON format of protocol object
                    string protocol = JsonConvert.SerializeObject(protocolObject);

                    try
                    {
                        // Encoding
                        int byteCount = Encoding.UTF8.GetByteCount(protocol);
                        byte[] sendDataa = new byte[byteCount];
                        sendDataa = Encoding.UTF8.GetBytes(protocol);

                        // write message to stream
                        stream.Write(sendDataa, 0, sendDataa.Length);

                        // flush when done
                        stream.Flush();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Message could not be sent..");
                    }
                }  
            }
        }

        // create protocol to send to server
        static void createProtocol(CreateProtocol protocolObject)
        {
            // Retrive Date and time
            DateTime dateTime = new DateTime();
            dateTime = DateTime.Now;
            String S_DateTime = dateTime.ToString();

            // Get my own IP-ADRESS automatically
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            // Add data to protcol object
            protocolObject.date = S_DateTime;
            protocolObject.senderIP = myIP;
            protocolObject.nickname = "Linda";

        }

        // send data to database, Code used in server not here.
        static void sendToDB(string message)
        {
            // connect to DB
            string server = "localhost";
            string userid = "root";
            string password = "Pa55word";
            string database = "kurs1";

            // connection string
            string cs = "server=" + server + ";userid=" + userid + ";password=" + password + ";database=" + database;

            // Loop here

            // parse info
            sendInfo info = JsonConvert.DeserializeObject<sendInfo>(message);

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

            // close connection
            con.Close();

        }

    }
}

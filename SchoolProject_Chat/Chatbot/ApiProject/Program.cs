using System;
using System.Threading.Tasks;
using System.Net; // dns
using System.Net.Http; // api
using Newtonsoft.Json; // json
using System.Net.Sockets;
using System.Text; // encoding
using System.Threading;

namespace ApiProject
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

    class Program
    {
        static async Task Main()
        {
            // Ip-adress to server, port 8080
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
                Thread ListenThr = new Thread(() => ListenThread(client, stream));
                Task SendThr = new Task(async()  => await SendThread(serverIP, client, stream));

                // start threads
                ListenThr.Start();
                SendThr.Start();

            }
            catch (Exception)
            {
                Console.WriteLine("Could not connect");
            }
        }

        // Send messages to server
        static async Task SendThread(string serverIP, TcpClient client, NetworkStream stream)
        {
            while (true)
            {
                // Wait 60 sek before sending joke
                //Thread.Sleep(60000);
                Thread.Sleep(5000);

                // create protocolobject
                CreateProtocol protocolObject = new CreateProtocol();

                // Fill protocol object with data
                createProtocol(protocolObject);
                protocolObject.recipientIP = serverIP;

                // get data from api
                await getDataFromApi(protocolObject);

                // make protocol object to JSON
                string protocol = JsonConvert.SerializeObject(protocolObject);

                // display message, debug
                Console.Write("'chatbot' says > ");
                Console.WriteLine(protocol);

                // Send message to Server
                try
                {
                    // encoding
                    int byteCount = Encoding.ASCII.GetByteCount(protocol);
                    byte[] sendDataa = new byte[byteCount];
                    sendDataa = Encoding.ASCII.GetBytes(protocol);

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

        // Get data from api
        static async Task getDataFromApi(CreateProtocol protocolObject)
        {
            // Create a HttpClient
            HttpClient client = new HttpClient();

            // Jokes from Open API
            string uri = "https://sv443.net/jokeapi/category/Miscellaneous";

            try
            {
                // Request data from URI, await task
                string responseBody = await client.GetStringAsync(uri);

                // parse data
                dynamic stuff = JsonConvert.DeserializeObject(responseBody);

                // To see if it is a 2 part joke or a Singel line joke
                string type = stuff.type;
                string message = "";

                if (type == "twopart")
                {
                    // get joke
                    string jokeSetup = stuff.setup;
                    string jokeDelivery = stuff.delivery;

                    // append to create message with a endline
                    message += stuff.setup;
                    message += " \n - ";
                    message += stuff.delivery;

                }
                else
                {
                    // get joke
                    string joke = stuff.joke;
                    message += stuff.joke;
                }

                // Add joke (message) into protocol object
                protocolObject.message = message;
         
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        // Create right protocol to send to server
        static void createProtocol(CreateProtocol protocolObject)
        {
            // Retrive Date and time now
            DateTime dateTime = new DateTime();
            dateTime = DateTime.Now;
            String S_DateTime = dateTime.ToString();

            // Get my own IP-ADRESS
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            // Add this data to protocol object
            protocolObject.date = S_DateTime;
            protocolObject.senderIP = myIP;
            protocolObject.nickname = "Chatbot";    

        }

        // Listen for messages, debug
        static void ListenThread(TcpClient client, NetworkStream stream)
        {
            while (true)
            {
                // if there is any data in stream
                if (stream.DataAvailable)
                {
                    // used to extract data
                    byte[] recievedBuffer = new byte[400];
                    StringBuilder msg = new StringBuilder();

                    // read from stream into recievedBuffer
                    try
                    {
                        stream.Read(recievedBuffer, 0, recievedBuffer.Length);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Stream could not be read..");
                    }

                    // for each byte in recievedBuffer, append to msg
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

                    // display message
                    string message = msg.ToString();
                    Console.WriteLine("\n" + message);

                    // flush stream when done
                    stream.Flush();
                }
            }
        }

    }
}

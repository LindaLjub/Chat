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
    // protocol
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
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();
        static string uri = "https://sv443.net/jokeapi/category/Miscellaneous";

        // chuck norris joke https://api.chucknorris.io/jokes/random
        // random daily joke https://api.jokes.one/jod.json
        // random good jokes https://sv443.net/jokeapi/category/Miscellaneous

        static async Task Main()
        {
            while(true)
            {
                Thread.Sleep(5000);

                // create protocolobject
                CreateProtocol protocolObject = new CreateProtocol();

                // Fill protocol with data
                createProtocol(protocolObject);

                //// get data from api
                //await getDataFromApi(protocolObject);

                // make protocol object to JSON
                string protocol = JsonConvert.SerializeObject(protocolObject);

               // Console.WriteLine(protocol);

                // send message
                MessageThread(protocol);
            }
        }

        // get data from api
        static async Task getDataFromApi(CreateProtocol protocolObject)
        {
            try
            {
                //HttpResponseMessage response = await client.GetAsync(uri);
                //response.EnsureSuccessStatusCode();
                //string responseBody = await response.Content.ReadAsStringAsync();

                // Above three lines can be replaced with new helper method below
                string responseBody = await client.GetStringAsync(uri);

                // parse
                dynamic stuff = JsonConvert.DeserializeObject(responseBody);

                // To see if it is a 2 part joke
                string type = stuff.type;
                string message = "";

                if (type == "twopart")
                {
                    string jokeSetup = stuff.setup;
                    string jokeDelivery = stuff.delivery;
                    // Console.WriteLine(jokeSetup + "\n" + jokeDelivery);

                    message += stuff.setup;
                    message += " \n";
                    message += stuff.delivery;

                }
                else
                {
                    string joke = stuff.joke;
                    // Console.WriteLine(joke);

                    message += stuff.joke;
                }

                // create message JSON
                protocolObject.message = message;
         
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        // protocol
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
            protocolObject.nickname = "Chatbot";
            protocolObject.recipientIP = "172.16.117.80";

        }

        // send message
        static void MessageThread(string protocol)
        {
            string serverIP = "172.16.117.80";
            int port = 8080;

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

                // stream.Close();
                // clienta.Close();
                stream.Flush();

            }
            catch (Exception)
            {
                Console.WriteLine("Message could not be sent..");
            }
        }
    }
}

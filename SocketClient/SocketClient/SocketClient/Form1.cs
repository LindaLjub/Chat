using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SocketClient
{

    public partial class Form1 : Form
    {
        string serverIP = "172.16.117.71";
        int port = 8080;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // SEND DATA
            TcpClient client = new TcpClient(serverIP, port);
            int byteCount = Encoding.ASCII.GetByteCount(message.Text);

            byte[] sendData = new byte[byteCount];

            sendData = Encoding.ASCII.GetBytes(message.Text);
            NetworkStream stream = client.GetStream();
            stream.Write(sendData, 0, sendData.Length);

            // RESPONSE
            // Buffer to store the response bytes.
            sendData = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(sendData, 0, sendData.Length);
            responseData = System.Text.Encoding.ASCII.GetString(sendData, 0, bytes);
            message.Text = (responseData);

            stream.Close();
            client.Close();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}

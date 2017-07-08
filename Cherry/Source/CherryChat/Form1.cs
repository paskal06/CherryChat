using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
namespace CherryChat
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //SET UP SOCKET
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            //Get User IP automatically:
            Local_IP.Text = GetLocalIP();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //Binding Socket
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(Local_IP.Text), Convert.ToInt32(Local_port.Text));
                sck.Bind(epLocal);
                //Connecting to remote IP
                epRemote = new IPEndPoint(IPAddress.Parse(Remote_IP.Text), Convert.ToInt32(Remote_port.Text));
                sck.Connect(epRemote);
                //Listening the specific port
                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None,ref epRemote,new AsyncCallback(MessageCallBack),buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Items.Add("System: !@ Check your input(IP, port, etc)");
            }
            
            
        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach(IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        private void Send_Click(object sender, EventArgs e)
        {
            //Convert msg to byte[]
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(textSend.Text);
            //Sending Encoded msg
            sck.Send(sendingMessage);
            MessageBox.Items.Add("Me: # " + textSend.Text);
            
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {


                byte[] receivedData = new byte[1500];
                receivedData = (byte[])aResult.AsyncState;
                //Converting byte[] to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedData);

                //Adding this message into ListBox
                MessageBox.Items.Add("Remote: $ " + receivedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception ex)
            {
                MessageBox.Items.Add("System: !@ "+ex.ToString());
            }
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Test_client
{

    class Client
    {
        IPHostEntry ipHostInfo;
        IPAddress ipAddress;
        IPEndPoint server;
        Socket socket;

        private bool running = false;
        byte[] bytes = new byte[2048];


        public Client(int port_)
        {
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            // ipAddress = ipHostInfo.AddressList[0];

            ipAddress = IPAddress.Parse("127.0.0.1");
            server = new IPEndPoint(ipAddress, port_);
            // Create a TCP/IP  socket.  
            socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {

            socket.Connect(server);
            Console.WriteLine("[CLIENT] I have connected to the server");
            string msg = "";

            while (running)
            {

                new Thread(() =>
                {
                    string data = "";
                    while (true)
                    {

                        int bytesReceived = socket.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesReceived);

                        if (data.IndexOf("<EOF>") > -1)
                        {
                            Console.WriteLine(data);
                        }
                    }
                }).Start();

                


                msg = Console.ReadLine();
                SendMessage(socket, msg);
            }

        }

        private void SendMessage(Socket s, string msg)
        {
            byte[] encodedMsg = Encoding.ASCII.GetBytes(msg + "<EOF>");
            s.Send(encodedMsg);
        }


    }


    public static void Main(string[] args)
    {
        Client client = new Client(11111);
        client.Start();
    }
}
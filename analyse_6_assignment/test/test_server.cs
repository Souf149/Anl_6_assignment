using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace test
{
    public class Program
    {

        public class Server
        {
            public Socket listener;
            public IPEndPoint localEndPoint;
            public IPAddress ipAddress;
            public int portNumber;
            public int listeningQueueSize = 5;

            private byte[] bytes = new byte[2048];
            private bool running = true;

            public Server(IPAddress ip_, int port_)
            {
                ipAddress = ip_;
                portNumber = port_;
                localEndPoint = new IPEndPoint(ipAddress, portNumber);
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // associate a network address to the Server Socket. All clients must know this address
                listener.Bind(localEndPoint);
                // This is a non-blocking listen with max number of pending requests
                listener.Listen(listeningQueueSize);
            }

            public void Start()
            {
                while (running)
                {
                    Socket connection = listener.Accept();
                    SendMessage(connection, Message.welcome);

                    string data = "";

                    // An incoming connection needs to be processed.  
                    while (true)
                    {

                        int bytesReceived = connection.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesReceived);

                        if (data.IndexOf("<EOF>") > -1)
                        {
                            // process message without <EOF>
                            string reply = ProcessMessage(data.Remove(data.Length - 5));

                            if(reply != "")
                            {
                                SendMessage(connection, $"I have received your message. It said: {reply}");
                            }
                            data = "";
                        }

                    }


                }


            }

            private string ProcessMessage(string data)
            {
                // TODO: Look what message is and do smthing accordingly
                return data;
            }

            private void SendMessage(Socket connection, string msg)
            {
                byte[] encodedMsg = Encoding.ASCII.GetBytes(msg + "<EOF>");
                connection.Send(encodedMsg);
            }


        }


        public static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Parse("127.0.0.1"), 11111);
            server.Start();

            Console.WriteLine();
            Console.Read();

        }

        public class ClientInfo
        {
            public string studentnr { get; set; }
            public string classname { get; set; }
            public int clientid { get; set; }
            public string teamname { get; set; }
            public string ip { get; set; }
            public string secret { get; set; }
            public string status { get; set; }
        }

        public class Message
        {
            public const string welcome = "WELCOME";
            public const string stopCommunication = "COMC-STOP";
            public const string statusEnd = "STAT-STOP";
            public const string secret = "SECRET";
        }

    }
}

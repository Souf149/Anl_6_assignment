/*** Fill these lines with your complete information.
 * Note: Incomplete information may result in FAIL.
 * Mameber 1: [First and Last name, first member]: // todo: write here.
 * Ahmet Karatas
 * Std Number 1: [Student number of the first member] // todo: write here.
 * 0963978
 * Class: [what is your class, example INF2C] // todo: write here.
 * INF2C
 ***/


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace SocketServer
{
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


    public class ConcurrentServer
    {
        public Socket listener;
        public IPEndPoint localEndPoint;
        public IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        public readonly int portNumber = 11111;

        public String results = "";
        public LinkedList<ClientInfo> clients = new LinkedList<ClientInfo>();

        private Boolean stopCond = false;
        private int processingTime = 1000;
        private int listeningQueueSize = 5;




        public ConcurrentServer()
        {
            Thread clientThread = new Thread(prepareServer);
            clientThread.Start();
        }

        // public void communicate()
        // {
        //     this.prepareServer();
        //     // this.exportResults();
        // }

        public void prepareServer()
        {
            byte[] bytes = new Byte[1024];
            String data = null;
            int numByte = 0;
            string replyMsg = "";
            bool stop;

            try
            {
                Console.WriteLine("[Server] is ready to start ...");
                // Establish the local endpoint
                localEndPoint = new IPEndPoint(ipAddress, portNumber);
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Console.Out.WriteLine("[Server] A socket is established ...");
                // associate a network address to the Server Socket. All clients must know this address
                listener.Bind(localEndPoint);
                // This is a non-blocking listen with max number of pending requests
                listener.Listen(listeningQueueSize);
                while (true)
                {
                    Console.WriteLine("Waiting connection ... ");
                    // Suspend while waiting for incoming connection 
                    Socket connection = listener.Accept();
                    this.sendReply(connection, Message.welcome);

                    stop = false;
                    while (!stop)
                    {
                        numByte = connection.Receive(bytes);
                        data = Encoding.ASCII.GetString(bytes, 0, numByte);
                        replyMsg = processMessage(data);
                        if (replyMsg.Equals(Message.stopCommunication))
                        {
                            stop = true;
                            break;
                        }
                        else
                            this.sendReply(connection, replyMsg);
                    }

                }

            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
            }
        }


        public string processMessage(String msg)
        {
            Thread.Sleep(processingTime);
            Console.WriteLine("[Server] received from the client -> {0} ", msg);
            string replyMsg = "";

            try
            {
                switch (msg)
                {
                    case Message.stopCommunication:
                        replyMsg = Message.stopCommunication;
                        break;
                    default:
                        ClientInfo c = JsonSerializer.Deserialize<ClientInfo>(msg.ToString());
                        clients.AddLast(c);
                        if (c.clientid == -1)
                        {
                            stopCond = true;
                            exportResults();
                        }
                        c.secret = c.studentnr + Message.secret;
                        c.status = Message.statusEnd;
                        replyMsg = JsonSerializer.Serialize<ClientInfo>(c);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Server] processMessage {0}", e.Message);
            }

            return replyMsg;
        }
        public void sendReply(Socket connection, string msg)
        {
            byte[] encodedMsg = Encoding.ASCII.GetBytes(msg);
            connection.Send(encodedMsg);
        }
        public void exportResults()
        {
            if (stopCond)
            {
                this.printClients();
            }
        }
        public void printClients()
        {
            string delimiter = " , ";
            Console.Out.WriteLine("[Server] This is the list of clients communicated");
            foreach (ClientInfo c in clients)
            {
                Console.WriteLine(c.classname + delimiter + c.studentnr + delimiter + c.clientid.ToString());
            }
            Console.Out.WriteLine("[Server] Number of handled clients: {0}", clients.Count);

            clients.Clear();
            stopCond = false;

        }


    }

    public class ServerSimulator
    {
        public static Thread[] myThreads;

        public static void concurrentRun()
        {

            Console.Out.WriteLine("[Server] A sample server, concurrent version ...");
            ConcurrentServer server = new ConcurrentServer();
            var length = server.clients.Count;
            var clients = server.clients;
            // Create threads

            // for (int i = 0; i < length; i++)
            // {
            //     myThreads[i] = new Thread(() => server.communicate());
            // }

            // for (int i = 0; i < length; i++)
            // {
            //     myThreads[i].Start();

            // }
            // for (int i = 0; i < length; i++)
            // {
            //     myThreads[i].Join();

            // }
        }
    }
    class Program
    {
        // Main Method 
        static void Main(string[] args)
        {
            Console.Clear();
            ServerSimulator.concurrentRun();
        }

    }
}

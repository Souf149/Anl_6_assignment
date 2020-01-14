using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;

class Program {

    static bool running = false;
    static List<ClientInfo> clients = new List<ClientInfo>();
    static List<Thread> threads = new List<Thread>();


    static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    static int portNumber = 11111;
    static IPEndPoint localEndPoint = new IPEndPoint(ipAddress, portNumber);

    // server
    static Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);


    public static void StartServer() {
        running = true;

        listener.Bind(localEndPoint);
        listener.Listen(5);
        Console.WriteLine("Server is up!");
        try {
            while (running) {
                Socket connection = listener.Accept();

                Socket s = connection;

                threads.Add(
                    new Thread(() => HandleClient(s))
                );



                threads[threads.Count - 1].Start();

            }

        }catch {
            Console.WriteLine("Server has been stopped");
            return;
        }
    }


    private static void HandleClient(Socket connection) {
        Console.WriteLine("Client connected!");
        SendMessage(connection, Message.welcome);

        string receivedMessage;
        byte[] bytes = new byte[1024];


        try {
            while (true) {
                int bytesReceived = connection.Receive(bytes);
                receivedMessage = Encoding.ASCII.GetString(bytes, 0, bytesReceived);

                string replyMessage = processMessage(receivedMessage);
                Console.WriteLine("I got message: " + receivedMessage);

                if (replyMessage == Message.stopCommunication) {
                    Console.WriteLine("I got a stop message!");
                    break;
                }

                SendMessage(connection, replyMessage);

                if (!running) {
                    Thread.Sleep(1000);
                    listener.Close();
                }

            }
        }
        catch {
            Console.WriteLine("ERROR: The client probably forced to close.");
        }


    }

    private static void FinishServer() {
        Console.WriteLine("Server is done");

        foreach (Thread thread in threads) {
            thread.Join();
        }

        Console.Out.WriteLine("[Server] This is the list of clients communicated");
        foreach (ClientInfo c in clients) {
            Console.WriteLine(c.classname + " " + c.studentnr + " " + c.clientid.ToString());
        }
        Console.Out.WriteLine($"[Server] Number of handled clients: {clients.Count}");

    }

    public static string processMessage(string msg) {
        string replyMsg = "";

        try {
            switch (msg) {
                case Message.stopCommunication:
                    replyMsg = Message.stopCommunication;
                    break;
                default:
                    ClientInfo client = JsonSerializer.Deserialize<ClientInfo>(msg.ToString());



                    client.secret = client.studentnr + Message.secret;
                    client.status = Message.statusEnd;
                    clients.Add(client);

                    if (client.clientid == -1) {
                        running = false;
                        Console.WriteLine("Last client received");

                    }

                    replyMsg = JsonSerializer.Serialize(client);
                    break;
            }
        }
        catch (Exception e) {
            Console.Out.WriteLine("[Server] processMessage {0}", e.Message);
        }

        return replyMsg;
    }
    private static void SendMessage(Socket s, string msg) {
        byte[] encodedMsg = Encoding.ASCII.GetBytes(msg);
        s.Send(encodedMsg);
    }

    public static void Main(string[] args) {

        StartServer();

        FinishServer();


        Console.WriteLine("End of program reached.");
        Console.Read();

    }



    public class ClientInfo {
        public string studentnr { get; set; }
        public string classname { get; set; }
        public int clientid { get; set; }
        public string teamname { get; set; }
        public string ip { get; set; }
        public string secret { get; set; }
        public string status { get; set; }
    }

    public class Message {
        public const string welcome = "WELCOME";
        public const string stopCommunication = "COMC-STOP";
        public const string statusEnd = "STAT-STOP";
        public const string secret = "SECRET";
    }

}


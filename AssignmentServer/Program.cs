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
class ConcurrentServer
{

    public bool stop = false;
    public List<Thread> myThreads = new List<Thread>();
    public List<ClientInfo> clients = new List<ClientInfo>();
    public String data = null;

    public IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    public readonly int portNumber = 11111;
    public IPEndPoint localEndPoint;
    public Socket listener;


    public void PrepareServer()
    {
        localEndPoint = new IPEndPoint(ipAddress, portNumber);
        listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        stop = true;

        listener.Bind(localEndPoint);
        listener.Listen(5);
        Console.WriteLine("[Server] is ready to start ...");
        Console.WriteLine("[Server] will receive messages now.....");

        try
        {
            while (stop)
            {
                Socket connection = listener.Accept();
                myThreads.Add(
                    new Thread(() => HandleClient(connection))
                );
                myThreads[myThreads.Count - 1].Start();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }


    public void HandleClient(Socket connection)
    {
        Console.WriteLine("[Server] Client connected........");
        this.SendReply(connection, Message.welcome);

        byte[] bytes = new byte[1024];


        try
        {
            while (true)
            {
                int numByte = connection.Receive(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, numByte);
                string replyMsg = processMessage(data);
                Console.WriteLine("Message received: " + data);

                if (replyMsg.Equals(Message.stopCommunication))
                {
                    Console.WriteLine("[Server] Stop message received.....!");
                    break;
                }
                else
                {
                    this.SendReply(connection, replyMsg);
                }
                if (!stop)
                {
                    Thread.Sleep(1000);
                    listener.Close();
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }


    }

    private void FinishServer()
    {
        Console.WriteLine("Execution finished");

        foreach (Thread thread in myThreads)
        {
            thread.Join();
        }

        Console.Out.WriteLine("[Server] This is the list of clients communicated");
        foreach (ClientInfo c in clients)
        {
            Console.WriteLine(c.classname + " " + c.studentnr + " " + c.clientid.ToString());
        }
        Console.Out.WriteLine("[Server] Number of handled clients: {0}",clients.Count);

    }

    public string processMessage(string msg)
    {
        string replyMsg = "";

        try
        {
            switch (msg)
            {
                case Message.stopCommunication:
                    replyMsg = Message.stopCommunication;
                    break;
                default:
                    ClientInfo client = JsonSerializer.Deserialize<ClientInfo>(msg.ToString());

                    client.secret = client.studentnr + Message.secret;
                    client.status = Message.statusEnd;
                    clients.Add(client);

                    if (client.clientid == -1)
                    {
                        stop = false;
                        Console.WriteLine("This is the last client of today!");

                    }

                    replyMsg = JsonSerializer.Serialize(client);
                    break;
            }
        }
        catch (Exception e)
        {
            Console.Out.WriteLine("[Server] processMessage {0}", e.Message);
        }

        return replyMsg;
    }
    public void SendReply(Socket s, string msg)
    {
        byte[] encodedMsg = Encoding.ASCII.GetBytes(msg);
        s.Send(encodedMsg);
    }

    public static void Main(string[] args)
    {
        ConcurrentServer c = new ConcurrentServer();
        c.PrepareServer();
        c.FinishServer();
        Console.WriteLine("Program has been terminated......");
        Console.Read();

    }

}


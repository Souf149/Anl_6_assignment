using analyse_6_assignment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace anl_6_assignment_SOUF {
    public static class Server {

        const int maxClients = 250;
        static List<Thread> threads = new List<Thread>();
        public static List<string> clientInfos = new List<string>();



        private static void MsgConsole(string msg) {
            Console.WriteLine($"[SERVER]: {msg}");
        }

        private static byte[] MsgToBytes(string msg) {
            return Encoding.ASCII.GetBytes(msg);
        }

        private static string ReceiveMsg(Socket s) {
            // Data buffer 
            byte[] messageReceived = new byte[1024];

            // We receive the messagge using  
            // the method Receive(). This  
            // method returns number of bytes 
            // received, that we'll use to  
            // convert them to string 
            int byteRecv = s.Receive(messageReceived);
            return Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
        }


        public static void Start() {
            //src: https://www.geeksforgeeks.org/socket-programming-in-c-sharp/
            // Establish the local endpoint  
            // for the socket. Dns.GetHostName 
            // returns the name of the host  
            // running the application. 
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 1490);

            MsgConsole($"IP/PORT: {localEndPoint}");

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);



            // Using Bind() method we associate a 
            // network address to the Server Socket 
            // All client that will connect to this  
            // Server Socket must know this network 
            // Address 
            listener.Bind(localEndPoint);

            // Using Listen() method we create  
            // the Client list that will want 
            // to connect to Server 
            listener.Listen(maxClients);

            while (true) {

                MsgConsole("Waiting connection ... ");

                // Suspend while waiting for 
                // incoming connection Using  
                // Accept() method the server  
                // will accept connection of client 
                Socket clientSocket = listener.Accept();

                Thread t = new Thread(() => HandleClient(clientSocket));
                threads.Add(t);
                t.Start();
                

            }
        }

        private static void HandleClient(Socket clientSocket) {


            // STEP 2
            clientSocket.Send(MsgToBytes("Welcome to the server!"));


            // STEP 4
            string string_json = ReceiveMsg(clientSocket);
            JSONMessage json = JsonConvert.DeserializeObject<JSONMessage>(string_json);
            json.secret = "jfj";
            json.status = "END";
            string msg = JsonConvert.SerializeObject(json);

            clientInfos.Add(msg);
            MsgConsole(msg);
            if(json.clientid != -1) {
                clientSocket.Send(MsgToBytes(msg));
            }
            else {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                foreach(string s in clientInfos) {
                    Console.WriteLine(s);
                }
            }

            





        }
    }


}



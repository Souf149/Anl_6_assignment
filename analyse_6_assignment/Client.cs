using analyse_6_assignment;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace anl_6_assignment_SOUF {
    public static class Client {

        private static void MsgConsole(string msg) {
            Console.WriteLine($"[CLIENT]: {msg}");
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

        private static void SendMsg(Socket s, string msg) {
            s.Send(Encoding.ASCII.GetBytes(msg));
        }
            
        

        public static void Start() {

            //src: https://www.geeksforgeeks.org/socket-programming-in-c-sharp/

            // Establish the remote endpoint  
            // for the socket. This example  
            // uses port 11111 on the local  
            // computer. 
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 1490);

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            Socket s = new Socket(ipAddr.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);

            // STEP 1
            // Connect Socket to the remote  
            // endpoint using method Connect() 
            s.Connect(localEndPoint);


            MsgConsole($"Received message: '{ReceiveMsg(s)}'");



            // STEP 3
            JSONMessage json = new JSONMessage() {
                studentnr = "0963595",
                classname = "INF2A",
                clientid = 1,
                teamname = "TEAM UMARU",
                ip = ipAddr.ToString()
            };
            string msg = JsonConvert.SerializeObject(json);
            SendMsg(s, msg);
            
        }


    }
}

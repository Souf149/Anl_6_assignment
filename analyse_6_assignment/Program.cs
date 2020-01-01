using anl_6_assignment_SOUF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace analyse_6_assignment {
    class Program {
        
        static void Main(string[] args) {
            while (true) {
                Console.WriteLine("Are you SERVER(1) or CLIENT(2)? \n");
                string input = Console.ReadLine();
                if (input == "1") {
                    SocketServer.Program.Run();
                    break;
                }else if (input == "2") {
                    SocketClient.Program.Run();
                    break;
                }
                else {
                    Console.WriteLine("Enter only 1 or 2");
                }
            }

            Console.WriteLine("End of program reached.");
            Console.ReadLine();
            
            


        }

    }

    class JSONMessage {
        public string studentnr = "";
        public string classname = "";
        public int clientid;
        public string teamname = "";
        public string ip = "";
        public string secret = "";
        public string status = "";
    }

}

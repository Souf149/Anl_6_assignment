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

            
            Thread T_server = new Thread(Server.Start);
            Thread T_client = new Thread(Client.Start);

            T_client.Start();
            T_server.Start();
            


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

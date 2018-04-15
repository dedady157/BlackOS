using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using BlackOS.CMD;

namespace BlackOS.Server
{
    struct Connection
    {
        public Socket Soc;
        public byte[] buffer;
        public ManualResetEvent allDone;
    }

    public class ControllServer
    {

        private Socket LIS;
        private Thread ACPThread;
        private Action<Socket> ProcessProc;

        ~ControllServer()
        {
            LIS.Dispose();
            ACPThread.Abort();
        }
        public ControllServer()
        {
            LIS = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LIS.Bind(new IPEndPoint(IPAddress.Any, 2460));
            ACPThread = new Thread(new ThreadStart(THR));
            ACPThread.Name = "Server.ACPThread";
            ACPThread.Priority = ThreadPriority.Highest;
            ProcessProc = new Action<Socket>(ProcessConn);
        }
        public void StartServer()
        {
            try
            {
                LIS.Listen(20);
                ACPThread.Start();
            }
            catch
            {
                Console.WriteLine("Failed to StartServer");
                Environment.Exit(ErrorCodes.Failed_ToStartServer);
            }
        }
        private void THR()
        {
            while(!Program.SHUTDOWN)
            {
                try
                {
                    Connection NewConn = new Connection() { allDone = new ManualResetEvent(false), buffer = new byte[10] };
                    LIS.BeginAccept(new AsyncCallback(ProcProcessingProcess), NewConn);
                    NewConn.allDone.WaitOne();
                }
                catch
                {
                    Console.WriteLine("ERROR Hit on Server.ACPThread");
                }
            }
        }
        private void ProcProcessingProcess(IAsyncResult R)
        {
            try
            {
                Connection conn = (Connection)R.AsyncState;
                conn.allDone.Set();
                conn.Soc = LIS.EndAccept(R);
                Thread Proc = new Thread(new ParameterizedThreadStart(ProcessConn));
                Proc.Start(conn);
            }
            catch
            {
                Console.WriteLine("Failed To Proc Processing Process in Server.ACPThread");
            }
        }
        private void ProcessConn(Object OBJ_conn)
        {
            Connection conn = (Connection)OBJ_conn;
            Console.WriteLine("processing command from: " + (conn.Soc.RemoteEndPoint as IPEndPoint).Address);
            conn.Soc.BeginReceive(conn.buffer, 0, 10, 0,new AsyncCallback(ReadAndProcessProcPacket), conn);
        }
        private void ReadAndProcessProcPacket(IAsyncResult R)
        {
            Connection conn = (Connection)R.AsyncState;
            while (!R.IsCompleted) ;
            conn.Soc.EndReceive(R);
            UInt16 ExecCode = BitConverter.ToUInt16(conn.buffer, 0);
            if (CommandsList.VerifyCommandExists(ExecCode))
            {
                Int64 ArgsSize = BitConverter.ToInt64(conn.buffer, 2);
                string AsciiArgs = "";
                if (ArgsSize > 0)
                {
                    conn.buffer = new byte[ArgsSize];
                    conn.Soc.Receive(conn.buffer);
                    AsciiArgs = Encoding.ASCII.GetString(conn.buffer);
                }

                conn.Soc.Send(BitConverter.GetBytes(ServerCodes.Success));
                ReturnStream RS = new ReturnStream(conn.Soc);
                Console.WriteLine("Executing command: "+ExecCode);
                CommandsList.Execute(ExecCode, AsciiArgs, RS);
                Console.WriteLine("CommandExitted");
                RS.Close();
            }
            else
            {
                conn.Soc.Send(BitConverter.GetBytes(ServerCodes.InvalidCommandCode));
            }
        }
        private void ReadFollowUpPacketCallBack(IAsyncResult R)
        {
            while (!R.IsCompleted) ;
            ((Connection)R.AsyncState).allDone.Set();
        }
    }
}

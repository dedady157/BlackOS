using System.Text;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;

namespace BlackOS.Server
{
    public class ReturnStream : Stream
    {
        private Stream BaseStream;
        private Socket Soc;
        internal string Tag;

        //T & F are just used to hard wire the return
        private bool F = false;
        private bool T = true;
        public override bool CanRead => F;
        public override bool CanSeek => F;
        public override bool CanWrite => T;
        public override long Length => throw new InvalidOperationException();
        public override long Position { get => throw new InvalidOperationException();
                                        set => throw new InvalidOperationException(); }

        ~ReturnStream()
        {
            Close();
        }
        public ReturnStream(Socket Soc) : base()
        {
            BaseStream = new NetworkStream(Soc);
            this.Soc = Soc;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Write(Encoding.Unicode.GetString(buffer));
        }//Jump to Write, ConversionType Unicode
        public void Write(string Text)
        {
            byte[] buff = Encoding.ASCII.GetBytes($"[{Tag}:{Thread.CurrentThread.ManagedThreadId}]:{Text}\n");
            BaseStream.Write(buff, 0, buff.Length);
        }
        public override void Close()
        {
            try
            {
                Console.WriteLine("Sending Kill con to " + (Soc.RemoteEndPoint as IPEndPoint).Address);
                BaseStream.Write(new byte[1] { 4 },0,1);
                BaseStream.Dispose();
                Soc.Disconnect(false);
                Soc.Dispose();
            }
            catch
            {

                if (!Program.SHUTDOWN)
                    Console.WriteLine("Failed to Close ReturnStream");
            }
        }

        //un used functions
        public override void Flush()
        {
            throw new InvalidOperationException();
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }
        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }
        
    }
}

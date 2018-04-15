using System;
using System.Text;
using System.IO;

namespace BlackOSPluginSDK.BlackOSPortal
{
    public class ReturnStream
    {
        internal Stream TrueReturnStream;
        public ReturnStream(Stream STM)
        {
            if (STM.CanRead == false && STM.CanSeek == false && STM.CanWrite == true)
                TrueReturnStream = STM;
            else throw new ArgumentException("Invalid Stream this does not contain a ReturnStream");
            
        }
        public void Write(string Text)
        {
            TrueReturnStream.Write(Encoding.Unicode.GetBytes(Text),0,0);
        }
    }
}

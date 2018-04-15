using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackOSPluginSDK.BlackOSPortal;

namespace BlackOSPluginSDK
{
    public struct CommandArgs
    {
        public string[] Args;
        public ReturnStream returnStream;
    }
    public static class CMD
    {
        public static bool Initulized { get; private set; } = false;
        private static CMDHandler Handle;
        public static void INIT(CMDHandler CMDHandler)
        {
            if (!Initulized)
            {
                Handle = CMDHandler;
                Initulized = true;
            }
        }
        public static void CreateCommand(UInt16 CID, Action<CommandArgs> CommandFunction, string Name, string OnErrorHelpText)
        {
            Handle._CreateCommandWayA.Invoke(CID, CommandFunction, Name, OnErrorHelpText);
        }
        public static void CreateCommand(UInt16 CID, Action<CommandArgs> CommandFunction, string Name)
        {
            Handle._CreateCommandWayB.Invoke(CID, CommandFunction, Name);
        }
        public static UInt16 CreateID()
        {
            return Handle._CreateID.Invoke();
        }
    }
}

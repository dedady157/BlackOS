using System;
using System.Collections.Generic;
using BlackOSPluginSDK;
namespace BlackOS.CMD
{

    public static class CommandsList
    {
        public static void INIT()
        {
            Commands = new Dictionary<ushort, Command>();
            CreateCommand(0, new Action<CommandArgs>(GetCommandsList), "ListCommands");
        }
        private static void GetCommandsList(CommandArgs x)
        {
            void Say(string Text) { x.returnStream.Write(Text); }

            Say("Name-----------------------------------ID---");
            Say("############################################");

            foreach(UInt16 key in Commands.Keys)
            {
                string name = Commands[key].name;
                for(int y = name.Length; y < 37; y++){name += " ";}
                name += "|";
                Say(name + key);
            }
            Say("############################################");
        }

        struct Command
        {
            public Action<CommandArgs> FunctionLink;
            public string name;
        }
        private static Dictionary<UInt16, Command> Commands;

        public static void CreateCommand(UInt16 CommandID, Action<CommandArgs> FunctionLink, string Name, string OnErrorHelpText)
        {
            if (string.IsNullOrEmpty(Name) || Name.Contains(" "))
            {
                Console.WriteLine("Failed To Hook Command, Command Name is invalid");
            }
            else
            {
                bool inuse = false;
                foreach (Command x in Commands.Values)
                {
                    if (x.name == Name) { inuse = true; break; }
                }
                if (inuse)
                {
                    Console.WriteLine("Failed To Hook Command, Command Name is inuse");
                }
                else if (Commands.ContainsKey(CommandID))
                {
                    Console.WriteLine("Failed To Hook Command, Command ID is in use");
                    if (!string.IsNullOrEmpty(OnErrorHelpText))
                    {
                        Console.WriteLine("HelpText:" + OnErrorHelpText);
                    }
                }
                else if (FunctionLink != null)
                {
                    Commands.Add(CommandID, new Command() { name = Name, FunctionLink = FunctionLink });
                }
                else
                {
                    Console.WriteLine("Failed To Hook Command, FunctionLink is invalid");
                    if (!string.IsNullOrEmpty(OnErrorHelpText))
                    {
                        Console.WriteLine("HelpText:" + OnErrorHelpText);
                    }
                }
            }
        }
        public static void CreateCommand(UInt16 CommandID, Action<CommandArgs> FunctionLink, string Name)
        {
            if (string.IsNullOrEmpty(Name) || Name.Contains(" "))
            {
                Console.WriteLine("Failed To Hook Command, Command Name is invalid");
            }
            else
            {
                bool inuse = false;
                foreach(Command x in Commands.Values)
                {
                    if (x.name == Name) { inuse = true; break; }
                }
                if(inuse)
                {
                    Console.WriteLine("Failed TO Hook Command, Command Name is inuse");
                }
                else if (Commands.ContainsKey(CommandID))
                {
                    Console.WriteLine("Failed To Hook Command, Command ID is in use");
                }
                else if (FunctionLink != null)
                {
                    Commands.Add(CommandID, new Command() { name = Name, FunctionLink = FunctionLink });
                }
                else
                {
                    Console.WriteLine("Failed To Hook Command, FunctionLink is invalid");
                }
            }
        }
        public static bool VerifyCommandExists(UInt16 CommandID)
        {
            return Commands.ContainsKey(CommandID);
        }
        public static void Execute(UInt16 CommandID, string Args, Server.ReturnStream RS)
        {
            if (Commands.ContainsKey(CommandID))
            {
                RS.Tag = Commands[CommandID].name;
                try
                {
                    Commands[CommandID].FunctionLink.Invoke(new CommandArgs() { returnStream = new BlackOSPluginSDK.BlackOSPortal.ReturnStream(RS), Args = Args.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) });
                }
                catch(Exception e)
                {
                    Console.WriteLine("Command Experianced Issues");
                    Console.WriteLine("Command:" + RS.Tag);
                    Console.WriteLine("Args:" + Args);
                    Console.WriteLine("Exception");
                    Console.WriteLine(e.Message);
                    RS.Write("Exception Thrown!!");
                    RS.Write(e.Message);
                }
             }
        }
        public static UInt16 CreateID()
        {
           for(UInt16 Key= 1; Key < UInt16.MaxValue;Key++)
            {
                if (!Commands.ContainsKey(Key)) { return Key; }
            }
            return 0;
        }

        //ForPlugins
        public static BlackOSPluginSDK.BlackOSPortal.CMDHandler CreateCMDHandlerForSDK()
        {
            BlackOSPluginSDK.BlackOSPortal.CMDHandler Handler;
            Handler = new BlackOSPluginSDK.BlackOSPortal.CMDHandler(new Action<ushort, Action<CommandArgs>, string, string>(CreateCommand), new Action<ushort, Action<CommandArgs>, string>(CreateCommand), new Func<ushort>(CreateID));
            return Handler;
        }
    }
}

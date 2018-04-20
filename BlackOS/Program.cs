#define DebuggModel

using System;
using Ev3DevLib;
using BlackOS.CMD;
using BlackOS.Server;
using BlackOS.PluginHandler;
using BlackOSPluginSDK;
using System.Threading;
using System.IO;

namespace BlackOS
{
    class Program
    {
        public static bool SHUTDOWN = false;
        static void Main(string[] args)
        {
            {
                string s = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(7);
                Environment.CurrentDirectory = s.Substring(0, s.LastIndexOf("/") + 2);//sets the WorkingDirectory to the program path
            }
            ControllServer Serv = null;
            //Console.TreatControlCAsInput=true;

            Console.Clear();
            Console.WriteLine("Verifying Enviroment");
            Console.WriteLine("WD:" + Environment.CurrentDirectory);
            if (!Directory.Exists(PluginSettings.PluginPath))
            {
                Directory.CreateDirectory(PluginSettings.PluginPath);
                Console.WriteLine("Created Plugin Directory");
            }
            Console.WriteLine("Done! Starting Program Boot");
            
            Console.WriteLine("Loading Services . . .");
            try
            {
                BrickControll.Brick.LEDS.SetBothLedsTo(LEDColor.red);
            }
            catch
            {
                Console.WriteLine("Failed to set LEDS from Ev3DevLib");
                Environment.Exit(ErrorCodes.Failed_ToLoadService);
            }//BrickControll.Brick.LEDS.SetBothLedsTo(LEDColor.red);
            try
            {
                CommandsList.INIT();
            }
            catch
            {
                Console.WriteLine("Failed to Initulize CommandList from BlackOS");
                Environment.Exit(ErrorCodes.Failed_ToLoadService);
            }//CommandsList.INIT();
            try
            {
                Serv = new ControllServer();
            }
            catch
            {
                Console.WriteLine("Failed to Load ControllServer from BlackOS");
                Environment.Exit(ErrorCodes.Failed_ToLoadService);
            }//Serv = new ControllServer();
            try
            {
                BlackOSPluginSDK.CMD.INIT(CMD.CommandsList.CreateCMDHandlerForSDK());
            }
            catch
            {
                Console.WriteLine("Failed to Initulize SDK CMDHandler from BlackOSPluginSDK");
                Environment.Exit(ErrorCodes.Failed_ToLoadService);
            }//BlackOSPluginSDK.CMD.INIT(CMD.CommandsList.CreateCMDHandlerForSDK());
            try
            {
                PluginController.INIT(PluginSettings.PluginPath);
            }
            catch
            {
                Console.WriteLine("Failed to Initulize PluginController from BlackOS");
                Environment.Exit(ErrorCodes.Failed_ToLoadService);
            }//PluginController.INIT(PluginSettings.PluginPath);
            Console.WriteLine("Services Loaded");
            
            Console.WriteLine("Hooking Devices . . .");
            try
            {
                BrickControll.LoadPorts();
            }
            catch
            {
                Console.WriteLine("Failed to load ports from Ev3DevLib");
                Environment.Exit(ErrorCodes.Failed_ToLoadService);
            }//BrickControll.LoadPorts();
            Console.WriteLine("Hooked Devices");

            Console.WriteLine("Hooking STD Commands . . .");
#if DebuggModel
            CommandsList.CreateCommand(CommandsList.CreateID(), new Action<CommandArgs>(ShutDown), "Shutdown", "This is a DebuggModel Command  /Shutdown");
            CommandsList.CreateCommand(CommandsList.CreateID(), new Action<CommandArgs>(TempCmd), "TestReciver", "This is a DebuggModel Command  /TestReciver");
#endif
            Ev3DevBotController.LoadHooks();
            Console.WriteLine("Hooked STD Commands");

            Console.WriteLine("Hook Plugin Commands");
            PluginController.LoadPluginHooks();
            Console.WriteLine("Hooked Plugin Commands");

            Console.WriteLine("Booting Services");
            try
            {
                Serv.StartServer();
            }
            catch
            {
                Console.WriteLine("Failed to Start ControllServer from BlackOS");
                Environment.Exit(ErrorCodes.Failed_ToStartServer);
            }//Serv.StartServer();
            Console.WriteLine("Fully Booted");

            Console.WriteLine("BlackOS Is Running . . .");
            while (!SHUTDOWN) ;

            Console.WriteLine("ShutdownCalled");

            Console.WriteLine("Stopping Plugins");
            PluginController.CleanUpPlugins();
            Console.WriteLine("Plugins Stopped, GoodBye!");
            Environment.Exit(0);

        }
        private static void ShutDown(CommandArgs x)
        {
            SHUTDOWN = true;
            x.returnStream.Write("Exitting BlackOS");
        }
        private static void TempCmd(CommandArgs CArgs)
        {
            void Say(string text) { CArgs.returnStream.Write(text); }

            Say("message 1");
            Thread.Sleep(10000);
            Say("message 2");
        }
    }
}

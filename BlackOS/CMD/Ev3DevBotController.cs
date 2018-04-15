using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackOS.CMD;
using BlackOSPluginSDK;
using System.Threading;
using Ev3DevLib;
using Ev3DevLib.Motors;
using Ev3DevLib.Sensors;
using System.Diagnostics;

namespace BlackOS
{
    public static class Ev3DevBotController
    {

        public static void LoadHooks()
        {
            CommandsList.CreateCommand(CommandsList.CreateID(), new Action<CommandArgs>(ChangeLeds), "ChangeLeds", "Ev3DevBotController.LoadHooks (ChangeLEDS)");
            CommandsList.CreateCommand(CommandsList.CreateID(), new Action<CommandArgs>(PrintDevices), "PrintDevices", "Ev3DevBotController.LoadHooks (PrintDevices)");
            CommandsList.CreateCommand(CommandsList.CreateID(), new Action<CommandArgs>(Cmd), "Cmd", "Ev3DevBotController.LoadHooks (Cmd)");
            CommandsList.CreateCommand(CommandsList.CreateID(), new Action<CommandArgs>(BotExecute), "BotExecute", "Ev3DevBotController.LoadHooks (BotExecute)");
            CommandsList.CreateCommand(CommandsList.CreateID(), new Action<CommandArgs>(UpdatePorts), "UpdatePorts", "Ev3DevBotController.LoadHooks (UpdatePorts)");
        }

        public static void Cmd(CommandArgs CArgs)
        {
            if (CArgs.Args.Length < 1)
            {
                CArgs.returnStream.Write(CommandSettings.Cmd.InvlidArgs);
            }
            else
            {
                Process P = new Process();
                P.StartInfo.FileName = CArgs.Args[0];
                P.StartInfo.UseShellExecute = true;
                if (CArgs.Args.Length >= 2)
                {
                    string Args = CArgs.Args[1];
                    for (int x = 2; x < CArgs.Args.Length; x++)
                    {
                        Args += " " + CArgs.Args[x];
                    }
                    P.StartInfo.Arguments = Args;
                }
                if (P.Start())
                {
                    CArgs.returnStream.Write(CommandSettings.Cmd.ProcessStarted);
                }
                else
                {
                    CArgs.returnStream.Write(CommandSettings.Cmd.ProcessFailedToStart);
                }
                if (P.WaitForExit(CommandSettings.Cmd.ProcessExitWaitTime))
                {
                    if (P.WaitForInputIdle(10))
                    {
                        CArgs.returnStream.Write(CommandSettings.Cmd.ProcessIsWaitingOnInput);
                        P.Close();
                    }
                    else
                    {
                        CArgs.returnStream.Write(CommandSettings.Cmd.ProcessTakesItsTime);
                        P.Kill();
                    }
                }
                else
                {
                    CArgs.returnStream.Write(CommandSettings.Cmd.ProcessClosed.Replace("%ExitCode%", P.ExitCode.ToString()));
                }
            }
        }
        public static void PrintDevices(CommandArgs CArgs)
        {
            void PrintDevice(Device D, string Dev)
            {
                CArgs.returnStream.Write($"Device {Dev}:");
                CArgs.returnStream.Write($"Connected:{D.Connected}");
                if (D.Connected)
                {

                    CArgs.returnStream.Write($"Type:{D._type}");
                    CArgs.returnStream.Write($"RootToDir:{D.RootToDir}");
                    if (D.Options != null)
                    {
                        string opt = D.Options[0];
                        for (int x = 1; x < D.Options.Length; x++)
                        {
                            opt += $", {D.Options[x]}";
                        }
                        CArgs.returnStream.Write($"Options:{opt}");
                    }
                }
                CArgs.returnStream.Write("");
                Thread.Sleep(100);
            }
            PrintDevice(BrickControll.OutputPorts.OutA, "OutA");
            PrintDevice(BrickControll.OutputPorts.OutB, "OutB");
            PrintDevice(BrickControll.OutputPorts.OutC, "OutC");
            PrintDevice(BrickControll.OutputPorts.OutD, "OutD");
            PrintDevice(BrickControll.InputPorts.In1, "In1");
            PrintDevice(BrickControll.InputPorts.In2, "In2");
            PrintDevice(BrickControll.InputPorts.In3, "In3");
            PrintDevice(BrickControll.InputPorts.In4, "In4");
        }
        private static void ChangeLeds(CommandArgs CArgs)
        {
            void Say(string text) { CArgs.returnStream.Write(text); }
            bool DisplayHT = false, ColorSet = false;
            LEDColor color = LEDColor.Green;


            for (int x = 0; x < CArgs.Args.Length; x++)
            {
                if (CArgs.Args[x] == "-h" || CArgs.Args[x] == "--help")
                {
                    DisplayHT = true;
                    break;
                }
                else if (!ColorSet)
                {
                    if (CArgs.Args[x] == "off")
                    {
                        color = LEDColor.off;
                        ColorSet = true;
                    }
                    else if (CArgs.Args[x] == "red")
                    {
                        color = LEDColor.red;
                        ColorSet = true;
                    }
                    else if (CArgs.Args[x] == "green")
                    {
                        color = LEDColor.Green;
                        ColorSet = true;
                    }
                    else if (CArgs.Args[x] == "orange")
                    {
                        color = LEDColor.orange;
                        ColorSet = true;
                    }
                    else
                    {
                        DisplayHT = true;
                        break;
                    }
                }
                else
                {
                    DisplayHT = true;
                    break;
                }
            }
            if (DisplayHT || !ColorSet)
            {
                Say("Uages is as follows");
                Say("ChangedLeds [color]");
                Say("Colors are:");
                Say("red");
                Say("green");
                Say("orange");
                Say("off");
                Say("Command Example");
                Say("ChangedLeds green");
                return;
            }
            else
            {
                BotHandler.ChangeLEDsTo(color);
                Say("Changed Led Color To " + color);
            }
        }
        private static void BotExecute(CommandArgs CArgs)
        {
            if (CArgs.Args.Length < 3 && !(CArgs.Args.Length > 1 && CArgs.Args[1] == "PrintOptions"))
            {
                CArgs.returnStream.Write("invalid Args i requre 3 args EG \"BotExecute <PortID> <Read/Write/PrintOptions> <CommandName - not if you PrintOptions> [CommandArgs]\"");
            }
            else
            {
                AutoOutPort OutP = null;
                AutoInPort InP = null;
                bool outputPort = false;
                bool inputPort = false;
                switch (CArgs.Args[0])
                {
                    case ("In1"):
                    case ("in1"):
                        InP = new AutoInPort(BrickControll.InputPorts.In1);
                        inputPort = true;
                        break;

                    case ("In2"):
                    case ("in2"):
                        InP = new AutoInPort(BrickControll.InputPorts.In2);
                        inputPort = true;
                        break;

                    case ("In3"):
                    case ("in3"):
                        InP = new AutoInPort(BrickControll.InputPorts.In3);
                        inputPort = true;
                        break;

                    case ("In4"):
                    case ("in4"):
                        InP = new AutoInPort(BrickControll.InputPorts.In4);
                        inputPort = true;
                        break;

                    case ("OutA"):
                    case ("outa"):
                        OutP = new AutoOutPort(BrickControll.OutputPorts.OutA);
                        outputPort = true;
                        break;

                    case ("OutB"):
                    case ("outb"):
                        OutP = new AutoOutPort(BrickControll.OutputPorts.OutB);
                        outputPort = true;
                        break; 

                    case ("OutC"):
                    case ("outc"):
                        OutP = new AutoOutPort(BrickControll.OutputPorts.OutC);
                        outputPort = true;
                        break;

                    case ("OutD"):
                    case ("outd"):
                        OutP = new AutoOutPort(BrickControll.OutputPorts.OutD);
                        outputPort = true;
                        break;

                    default:
                        CArgs.returnStream.Write("'Invalid PortID Options Are (\"in1\",\"in2\",\"in3\",\"in4\",\"outa\",\"outb\",\"outc\",\"outd\")");
                        break;
                }
                if(outputPort)
                {
                    if (CArgs.Args[1] == "Write")
                    {
                        int left = CArgs.Args.Length - 3;
                        string[] Args;
                        if (left > 0)
                        {
                            Args = new string[left];
                            Array.Copy(CArgs.Args, 3, Args, 0, left);
                        }
                        else Args = new string[0];

                        OutP.ExecuteWriteOption(CArgs.Args[2],Args);
                    }
                    else if (CArgs.Args[1] == "Read")
                    {
                        CArgs.returnStream.Write(OutP.ExecuteReadOption(CArgs.Args[2]));
                    }
                    else if (CArgs.Args[1] == "PrintOptions")
                    {
                        string Opt = $"\"{OutP.Options[0]}\"";
                        for(int x = 1; x < OutP.Options.Length;x++)
                        {
                            Opt += $", \"{OutP.Options[x]}\"";
                        }
                        CArgs.returnStream.Write("Options Are:" + Opt);
                    }
                    else
                    {
                        CArgs.returnStream.Write("Invalid Operation Type options are \"Read\",\"Write\",\"PrintOptions\"");
                    }
                }
                else if (inputPort)
                {
                    if (CArgs.Args[1] == "Write")
                    {
                        int left = CArgs.Args.Length - 3;
                        string[] Args;
                        if (left > 0)
                        {
                            Args = new string[left];
                            Array.Copy(CArgs.Args, 3, Args, 0, left);
                        }
                        else Args = new string[0];

                        InP.ExecuteWriteOption(CArgs.Args[2], Args);
                    }
                    else if (CArgs.Args[1] == "Read")
                    {
                        CArgs.returnStream.Write(InP.ExecuteReadOption(CArgs.Args[2]));
                    }
                    else if (CArgs.Args[1] == "PrintOptions")
                    {
                        string Opt = $"\"{InP.Options[0]}\"";
                        for (int x = 1; x < InP.Options.Length; x++)
                        {
                            Opt += $", \"{InP.Options[x]}\"";
                        }
                        CArgs.returnStream.Write("Options Are:" + Opt);
                    }
                    else
                    {
                        CArgs.returnStream.Write("Invalid Operation Type options are \"Read\",\"Write\",\"PrintOptions\"");
                    }
                }
            }
        }
        private static void UpdatePorts(CommandArgs CArgs)
        {
            CArgs.returnStream.Write("Updating Ports . . .");
            BrickControll.LoadPorts();
            CArgs.returnStream.Write("Done!");
        }
    }
}

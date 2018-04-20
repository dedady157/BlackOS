using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BlackOSPluginSDK;
namespace BlackOS.PluginHandler
{
    public static class PluginController
    {
        struct PreInitPluginInfo
        {
            public Type type;
            public string FileName;
        }
        struct PluginInfo
        {
            public IBlackOSPlugin Interface;
            public bool CommandsLoaded => Interface.CommandsHooked;
            public string Name => Interface.PluginName;
        }
        private static List<PluginInfo> Plugins = new List<PluginInfo>();
        public static void INIT(string PluginsPath)
        {
            //web code (Editted)
            string[] dllFileNames = null;
            if (Directory.Exists(PluginsPath))
            {
                dllFileNames = Directory.GetFiles(PluginsPath, "*.dll");
            }
            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (string dllFile in dllFileNames)
            {
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }
            Type pluginType = typeof(IBlackOSPlugin);
            ICollection<PreInitPluginInfo> plugins = new List<PreInitPluginInfo>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(pluginType.FullName) != null)
                            {
                                plugins.Add(new PreInitPluginInfo() { FileName = assembly.FullName, type = type });
                            }
                        }
                    }
                }
            }
            foreach (PreInitPluginInfo info in plugins)
            {
                IBlackOSPlugin plugin = (IBlackOSPlugin)Activator.CreateInstance(info.type);
                try
                {
                    plugin.INIT();
                    Plugins.Add(new PluginInfo() { Interface = plugin });
                }
                catch (Exception e)
                {
                    string Name = "Failed To Get Name";
                    string File = info.FileName;
                    try
                    {
                        Name = plugin.PluginName;
                    }
                    catch
                    {

                    }

                    Console.WriteLine("Failed to Initulize Plugin");
                    Console.WriteLine($"PluginName:{Name}");
                    Console.WriteLine($"FileName:{File}");
                    Console.WriteLine("Exception:");
                    Console.WriteLine(e.ToString());
                    if (e.Message != null)
                        Console.WriteLine($"Exception Message:{e.Message}");
                    else
                        Console.WriteLine("*No Exception Message*");

                }
            }
        }
        public static void LoadPluginHooks()
        {
            if (BlackOSPluginSDK.CMD.Initulized)
            {
                foreach (PluginInfo P in Plugins)
                {
                    try
                    {
                        if (!P.CommandsLoaded)
                            P.Interface.HookCommands();
                    }
                    catch
                    {
                        string Name = "Failed To Get Name";
                        try
                        {
                            Name = P.Name;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed to LoadHooks for Plugin");
                            Console.WriteLine($"PluginName:{Name}");
                            Console.WriteLine("Exception:");
                            Console.WriteLine(e.ToString());
                            if (e.Message != null)
                                Console.WriteLine($"Exception Message:{e.Message}");
                            else
                                Console.WriteLine("*No Exception Message*");
                        }
                    }
                }
            }
        }
        public static void CleanUpPlugins()
        {
            foreach (PluginInfo P in Plugins)
            {
                try
                {
                    P.Interface.DeINIT();
                }
                catch
                {
                    string Name = "Failed To Get Name";
                    try
                    {
                        Name = P.Name;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to DeINIT Plugin");
                        Console.WriteLine($"PluginName:{Name}");
                        Console.WriteLine("Exception:");
                        Console.WriteLine(e.ToString());
                        if (e.Message != null)
                            Console.WriteLine($"Exception Message:{e.Message}");
                        else
                            Console.WriteLine("*No Exception Message*");
                    }
                }
            }
        }
    }
}

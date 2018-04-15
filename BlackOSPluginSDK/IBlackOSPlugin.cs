using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackOSPluginSDK
{
    public interface IBlackOSPlugin
    {
        bool CommandsHooked { get; }
        string PluginName { get; }
        string Creator { get; }
        void INIT();
        void HookCommands();
        void DeINIT();
    }
}

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

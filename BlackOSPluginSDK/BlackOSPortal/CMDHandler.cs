using System;
namespace BlackOSPluginSDK.BlackOSPortal
{
    public struct CMDHandler
    {
        public Action<UInt16, Action<CommandArgs>, string, string> _CreateCommandWayA { get; private set; }
        public Action<UInt16, Action<CommandArgs>, string> _CreateCommandWayB { get; private set; }
        public Func<UInt16> _CreateID { get; private set; }
        
        public CMDHandler(Action<UInt16, Action<CommandArgs>, string, string> CCA, Action<UInt16, Action<CommandArgs>, string> CCB, Func<UInt16> CCID)
        {
            _CreateID = CCID;
            _CreateCommandWayA = CCA;
            _CreateCommandWayB = CCB;
        }
    }
}

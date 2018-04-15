using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackOS.CMD
{
    public static class CommandSettings
    {
        public static class Cmd
        {
            public const int ProcessExitWaitTime=10*1000;
            public const string ProcessFailedToStart = "Process Failed to Start";
            public const string ProcessStarted = "Process Started";
            public const string ProcessIsWaitingOnInput = "This process wants an input but this is not supported";
            public const string ProcessClosed = "Process has Exited With Code %ExitCode$";
            public const string ProcessTakesItsTime = "Process is taking its time and i dont know if its failing to do something so im just gonna kill it";
            public const string InvlidArgs = "This executes the command given";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackOS
{
    public static class ErrorCodes
    {
        public const int Failed_ToLoadService = 3;
        public const int Failed_ToLoadDevices = 4;
        public const int Failed_ToResetDevices = 5;
        public const int Failed_ToStartServer = 6;
        public const int Failed_ToProcAProcess = 7;

        public const int Success_ServiceStarted = 10;
        public const int Failed_ServiceFailedToStart = 11;
        public const int Warning_ServiceFailedToStopForcingExit = 12;
        public const int Failed_ServiceStructUnset = 13;
        public const int Success_ServiceStoped = 14;
        public const int Failed_ServiceWasPermenantlyStoped = 15;


    }
}

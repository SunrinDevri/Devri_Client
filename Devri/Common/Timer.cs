using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using System.IO;
using Sysem.Net;

namespace Devri.Common
{
    class Timer
    {
        public object DisPatcher { get; private set; }

        public void Timer_Start(int j)
        {
            TimeSpan  delay = TimeSpan.FromMinutes(j);
            ThreadPoolTimer  DelayTimer = ThreadPoolTimer.CreateTimer((source){DisPatcher},delay);
        }
    }
}

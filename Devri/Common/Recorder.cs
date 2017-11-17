using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Devri.Interact;
using Windows.Media.Capture;

namespace Devri.Common
{
    class Recorder
    {
        private System.Threading.Timer timer;
        public object DisPatcher { get; private set; }

        public void Timer_Start(int j)
        {
            TimeSpan delay = TimeSpan.FromMinutes(j);
            //j must be a millisecond integer
            timer = new System.Threading.Timer(timerCallback, null, 0, j);

        }

        private async void timerCallback(object state)
        {
            
            timer.Dispose();
            
        }



    }
}

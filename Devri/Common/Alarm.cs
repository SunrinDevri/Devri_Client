﻿using Devri.Interact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Devri.Common
{
    class Alarm
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
            //TTS.TTSPOSTAsync("");
            timer.Dispose();
            await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () => {
                // do some work on UI here;
            });
        }

    }
}

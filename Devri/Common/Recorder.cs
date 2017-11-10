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
        public event MediaCaptureFailedEventHandler Failed;
        static public async Task RecordStartAsync()
        {
            MediaCapture mediaCapture = new MediaCapture();
            bool isPreviewing;
            await mediaCapture.InitializeAsync();
            MediaCaptureFailedEventHandler MediaCapture_Failed = null;
            mediaCapture.Failed += MediaCapture_Failed;

        }




        public JObject SendSound()
        {
            String bep =ServerCommunication.POST_FILE("", "record.mp3");
            JObject sep = JObject.Parse(bep);
            return sep;
        }
    }
}

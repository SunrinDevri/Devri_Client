using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using System.IO;
using System.Net;
using Windows.UI.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Devri.Common
{
    class Timer_Devri
    {
        private System.Threading.Timer timer;
        public object DisPatcher { get; private set; }

        public static bool isTimerRun = false;




        public void Timer_Start(int j)
        {
            isTimerRun = true;
            TimeSpan  delay = TimeSpan.FromMinutes(j);
            //j must be a millisecond integer
            timer = new System.Threading.Timer(timerCallback, null,0,60000);
            
        }

        private async void timerCallback(object state)
        {

            DateTime dt = DateTime.Now;

            //30min
            dt.ToString();
            //Put Something you want to run async
            await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () => {
            // do some work on UI here;
            });
        }
        public JArray Schedule_List = new JArray();

        public void Add_Schedule(JObject Task)
        {
            JObject new_schedule = new JObject();
            DateTime dt = DateTime.Now;
            new_schedule.Add("Schedule_Name", Task["Keyword1"].ToString());
            new_schedule.Add("Schedule_Time", dt.AddMinutes(Double.Parse( Task["Keyword2"].ToString()) ));
            Schedule_List.Add(new_schedule);
            Save_Schedule(Schedule_List.ToString());
        }


        public void Save_Schedule(String savedata)
        {
            FileStream fs = new FileStream("Schedule.json", FileMode.Append);
            StreamWriter w = new StreamWriter(fs);


        }
        public JArray Load_Schedule()
        {
            FileStream fs = new FileStream("Schedule.json", FileMode.Append);
            List<JObject> ScriptList = new List<JObject>();
            String locate = Directory.GetCurrentDirectory();
            StreamReader file = File.OpenText("Schedule.json");
            var data = file.ReadToEnd();
            JArray Scedule = JArray.Parse(data);


            return Scedule;

        }
    }
}

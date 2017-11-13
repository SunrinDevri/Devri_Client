using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;


namespace Devri.Common
{
    class Schedule
    {
        public JArray Schedule_List = new JArray();

        public void Add_Schedule(JObject Task)
        {
            JObject new_schedule = new JObject();
            new_schedule.Add("Schedule_Name", Task["Keyword1"].ToString());
            Schedule_List.Add(new_schedule);

        }


        public void Save_Schedule()
        {
            FileStream fs = new FileStream("Schedule.json", FileMode.Append);
            StreamWriter w = new StreamWriter(fs);

            
        }
        public void Load_Schedule()
        {
            FileStream fs = new FileStream("Schedule.json", FileMode.Append);
            StreamReader sr = new StreamReader(fs);


            
        }

    }
}

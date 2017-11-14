using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Windows.UI.Xaml;
using Devri.Common;
using static Devri.Common.Feeling;
using System.Data;

namespace Devri.Interact
{
    class VoiceAct
    {
        public static String locate = @"Resource/Scripts.json";
        public String LineSelector (JObject reci) {
            List<JObject> ScriptList = new List<JObject>();
            Random rd = new Random();
            String locate = Directory.GetCurrentDirectory();
            locate = Path.GetDirectoryName(Path.GetDirectoryName(locate)) + @"/Scripts.json";
            StreamReader file = File.OpenText(locate);
            var data = file.ReadToEnd();
            JArray Scripts = JArray.Parse(data);
            foreach (JObject itemObj in Scripts.Children<JObject>())
            {
                if (itemObj["Situation1"].ToString() == reci["Situaion1"].ToString() &&(
                    (itemObj["Feeling"].Contains(feel_table[GetStatus()]))||itemObj["Feeling"].Equals("All")))
                {
                    ScriptList.Add(itemObj);
                }
                
            }
            //JObject ret = ScriptList[rd.Next(0, ScriptList.Count)];





            return "";

        }

        public String EventPlayer(JArray KJS,JObject reci)
        {
            Random rd = new Random();
            JObject ret = (JObject)KJS[rd.Next(0, KJS.Count)];
            String temp = ret["Situation1"].ToString();
            string result = "";
            switch (temp)
            {
                
                case "Schedule_Alarm":
                    break;


                




                case "Today":
                    result = ret["Line"].ToString();
                    break;
                    
                case "Weather":
                    JArray get_weather = JArray.Parse(ServerCommunication.GET("http://iwin247.kr:80/weather", "GPS+Now+DeviceID"));
                    break;

                case "Music_Recommand":
                    JObject get_music = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:80/music", "DeviceID"));
                    break;
                case "Temperature_Humidity_Management":
                    break;
                case "Movie_Recommand":
                    JObject get_Movie  = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:80/movie", "GPS+DeviceID"));
                    break;
                case "Book_Recommand":
                    JObject get_Book = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:80/book", "DeviceID"));
                    break;
                
                case "TodayWord":
                    JObject get_word = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:80/word", "DeviceID"));
                    break;
                case "D-day":

                    break;
                case "MoodLamp":
                    //led
                    break;           
                case "Status_Check":
                    result = ret["Line"].ToString();
                    
                    break;
                case "Disable":
                    result = ret["Line"].ToString();
                    //Disable Code
                    break;

                case "Traffic_Info":
                    JArray get_traffic = JArray.Parse(ServerCommunication.GET("http://iwin247.kr:80/traffic", reci["keyword1"].ToString()));
                    Event_Traffic_Info(ret, get_traffic,reci);
                    break;
                case "Calculator":
                    double Output =StringToFormula.Eval(reci["Keyword1"].ToString());
                    result = ret["Line"].ToString().Replace("{{Result}}", Output.ToString());
                    return result;
                    break;
                case "Timer":

                    
                    break;
               

            }
            
            return result;
        }
        public string Event_Schedule_Alarm(JObject asd, JObject reci)
        {
            return "";
        }
        //public string Event_Today(String asd)
        //{
        //    DateTime dt = DateTime.Now;
        //    string st = "";
        //    st = st + dt.Month.ToString()+"/"+dt.Day.ToString();
        //    return "";
        //}
        public string Event_Weather(JObject asd, JObject reci)
        {
            return "";
        }
        public string Event_Music_Recommand(JObject asd, JObject reci)
        {
            return "";
        }
        public string Event_Book_Recommand(JObject asd, JObject reci)
        {
            return "";
        }
        public string Event_TodayWord(JObject asd, JObject reci)
        {
            return "";
        }
        public string Event_MoodLamp(JObject asd, JObject reci)
        {
            /*램프*/
            return "";
        }
        public string Event_Disable(JObject asd, JObject reci)
        {
            /*오디오 비활성화*/
            return "";
        }
        public string Event_Traffic_Info(JObject asd,JArray get,JObject reci )
        {
            if (asd["Situation2"].ToString() == "HARD")
            {
                String Sectors = "";
                foreach (JObject itemObj in get.Children<JObject>())
                {
                    if (itemObj["Status"].ToString() == "정체")
                    {
                        Sectors = Sectors + itemObj[Sectors].ToString() + " ";
                    }

                }
                asd["Line"].ToString().Replace("{{Sector[]}}", Sectors);
                return asd["Line"].ToString().Replace("{{Road_Name}}", reci["Keyword1"].ToString());
            }
            else
            {

                return asd["Line"].ToString().Replace("{{Road_Name}}", reci["Keyword1"].ToString());
            }

            
        }
        public string Event_Timer(JObject asd, JObject reci)
        {
            Timer_Devri td = new Timer_Devri();
            int j =Int32.Parse(reci["Keyword1"].ToString());
            if (Timer_Devri.isTimerRun==false)
            {
                td.Timer_Start(j);
            }
            return "";
        }
    }
}

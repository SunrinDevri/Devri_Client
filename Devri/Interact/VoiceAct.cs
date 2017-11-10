using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Windows.UI.Xaml;
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
                if (itemObj["Situation1"].ToString() == reci["Situaion1"].ToString() && itemObj["Situation2"].ToString() == reci["Situaion2"].ToString())
                {
                    ScriptList.Add(itemObj);
                }
                
            }
            JObject ret = ScriptList[rd.Next(0, ScriptList.Count)];





            return "";

        }

        public String EventPlayer(JObject KJS)
        {
            String temp = KJS["Situation1"].ToString();
            switch (temp)
            {
                case "First Meet": break;
                case "Call":break;
                case "Alarm":break;
                case "Schedule_Alarm": break;
                case "Morning_greeting": break;
                case "Lunch_greeting": break;
                case "Dinner_greeting": break;
                case "Today": break;
                case "Weather": break;
                case "Music_Recommand": break;
                case "Temperature_Humidity_Management": break;
                case "Movie_Recommand": break;
                case "Book_Recommand": break;
                case "Time_Signal": break;
                case "TodayWord": break;
                case "D-day": break;
                case "MoodLamp": break;
                case "Status_Check":break;
                case "Disable": break;
                case "Traffic_Info": break;
                case "Calculator": break;
                case "Timer": break;
                case "EasterEgg": break;

            }
            //if (temp == "Schedule_Alarm")
            //{

            //}
            //else if (temp == "Weather")
            //{
            //    ServerCommunication.GET("iwin247.kr:80/music", "");
            //}
            //else if (ret["Situation1"].ToString() == "Music")
            //{
            //    ServerCommunication.GET("iwin247.kr:80/movie", "");
            //}
            //else if (ret["Situation1"].ToString() == "Movie_Recommand")
            //{
            //    ServerCommunication.GET("iwin247.kr:80/movie", "");
            //}
            //else if (ret["Situation1"].ToString() == "TodayWord")
            //{
            //    ServerCommunication.GET("iwin247.kr:80/movie", "");
            //}
            //else if (ret["Situation1"].ToString() == "Book_Recommand")
            //{
            //    ServerCommunication.GET("iwin247.kr:80/movie", "");
            //}
            //else if (ret["Situation1"].ToString() == "Calculator")
            //{
            //    ServerCommunication.GET("iwin247.kr:80/movie", "");
            //}
            //else if (ret["Situation1"].ToString() == "Timer")
            //{
            //    ServerCommunication.POSTAsync("iwin247.kr:80/timer", "");
            //}
            //else if (ret["Situation1"].ToString() == "Timer")
            //{
            //    ServerCommunication.GET("iwin247.kr:80/timer", reci["Keyword1"].ToString());
            //}
            //return "잘 모르겠습니다";
            return "";
        }
        public String Input(String line,string datalink)
        {
            

            String result="";
            return result;
        } 
    }
}

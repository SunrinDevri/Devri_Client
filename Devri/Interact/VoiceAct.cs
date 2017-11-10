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
           


            JObject ret =  ScriptList[rd.Next(0, ScriptList.Count)];
            if (ret["Situation1"].ToString() == "Schedule_Alarm")
            {
                ServerCommunication.GET("iwin247.kr:80/movie", "");
            }
            else if (ret["Situation1"].ToString() == "Weath-Recommand")
            {
                ServerCommunication.GET("iwin247.kr:80/music", "");
            }
            else if (ret["Situation1"].ToString() == "Music")
            {
                ServerCommunication.GET("iwin247.kr:80/movie", "");
            }
            else if (ret["Situation1"].ToString() == "Movie_Recommand")
            {
                ServerCommunication.GET("iwin247.kr:80/movie", "");
            }
            else if (ret["Situation1"].ToString() == "TodayWord")
            {
                ServerCommunication.GET("iwin247.kr:80/movie", "");
            }
            else if (ret["Situation1"].ToString() == "Book_Recommand")
            {
                ServerCommunication.GET("iwin247.kr:80/movie", "");
            }
            else if (ret["Situation1"].ToString() == "Calculator")
            {
                ServerCommunication.GET("iwin247.kr:80/movie", "");
            }
            else if (ret["Situation1"].ToString() == "Timer")
            {
                ServerCommunication.POSTAsync("iwin247.kr:80/timer", "");
            }
            else if (ret["Situation1"].ToString() == "Timer")
            {
                ServerCommunication.GET("iwin247.kr:80/timer", reci["Keyword1"].ToString());
            }

            return "";

        }
        public String Input(String line,string datalink)
        {
            

            String result="";
            return result;
        } 
    }
}

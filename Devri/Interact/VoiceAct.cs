﻿using System;
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
            locate = Path.GetDirectoryName(Path.GetDirectoryName(locate)) + @"Resources/Scripts.json";
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

        public void EventPlayer(JArray KJS,JObject reci)
        {
            Random rd = new Random();
            JObject ret = (JObject)KJS[rd.Next(0, KJS.Count)];
            String temp = ret["Situation1"].ToString();
            string result = "";
            switch (temp)
            {






                
                case "Schedule_Alarm":
                    Timer_Devri td = new Timer_Devri();
                    td.Add_Schedule(reci);
                    result = ret["Line"].ToString().Replace("{{Task_Name}}", reci["Keyword1"].ToString());


                    Feeling.SetXAxis(2);



                    break;


                




                case "Today":
                    result = ret["Line"].ToString();
                    TTS.TTSPOSTAsync(result);
                    Feeling.SetYAxis(2);
                    break;
                    
                case "Weather":
                    JArray get_weather = JArray.Parse(ServerCommunication.GET("http://iwin247.kr:3080/weather", "GPS+Now+DeviceID"));
                    foreach (JObject itemObj in get_weather.Children())
                    {
                        result = KJS["Line"].ToString().Replace("{{Weather_Now}}", itemObj["condition"].ToString());
                        result.Replace("{{Weather}}", itemObj[""].ToString());

                    }
                    Feeling.SetXAxis(2);
                    TTS.TTSPOSTAsync(result);
                    break;

                case "Music_Recommand":
                    JObject get_Music = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:3080/music", "DeviceID"));
                    result = KJS["Line"].ToString().Replace("{{Music}}", get_Music["name"].ToString());
                    result.Replace("{{Artist}}", get_Music["artist"].ToString());
                    TTS.TTSPOSTAsync(result);
                    Feeling.SetXAxis(2);
                    break;
                case "Movie_Recommand":
                    JObject get_Movie  = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:3080/movie", "DeviceID"));
                    result = KJS["Line"].ToString().Replace("{{Movie}}",get_Movie["name"].ToString());
                    TTS.TTSPOSTAsync(result);
                    Feeling.SetYAxis(2);
                    break;

                case "Book_Recommand":
                    JObject get_Book = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:3080/book", "DeviceID"));
                    result = KJS["Line"].ToString().Replace("{{Book_Title}}", get_Book["name"].ToString());
                    result.Replace("{{Author}}", get_Book["artist"].ToString());
                    TTS.TTSPOSTAsync(result);
                    Feeling.SetXAxis(2);
                    break;


                case "TodayWord":
                    JObject get_Word = JObject.Parse(ServerCommunication.GET("http://iwin247.kr:3080/word", "DeviceID"));
                    result = KJS["Line"].ToString().Replace("{{Word}}", get_Word["name"].ToString());
                    result.Replace("{{Author}}", get_Word["artist"].ToString()); TTS.TTSPOSTAsync(result);
                    Feeling.SetXAxis(2);
                    break;
                case "D-day":
                    


                    break;
                case "MoodLamp":
                    //led
                    result = ret["Line"].ToString();
                    TTS.TTSPOSTAsync(result);

                    break;           
                case "Status_Check":
                    result = ret["Line"].ToString();
                    TTS.TTSPOSTAsync(result);
                    //finished
                    break;
                case "Disable":
                    result = ret["Line"].ToString();
                    TTS.TTSPOSTAsync(result);
                    //Disable Code
                    break;

                case "Traffic_Info":
                    JArray get_traffic = JArray.Parse(ServerCommunication.GET("http://iwin247.kr:3080/traffic", reci["keyword1"].ToString()));
                    TTS.TTSPOSTAsync( Event_Traffic_Info(ret, get_traffic,reci)); 
                    //finished
                    break;
                case "Calculator":
                    double Output =StringToFormula.Eval(reci["Keyword1"].ToString());
                    result = ret["Line"].ToString().Replace("{{Result}}", Output.ToString());
                    TTS.TTSPOSTAsync(result); 
                    //finisihed
                    break;
                case "Timer":
                    TTS.TTSPOSTAsync(result);
                    break;



                default:
                    TTS.TTSPOSTAsync(ret["Line"].ToString());
                    //Set default action
                    break;
                    

            }
            
            
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
               //need new timer plz
            }
            return "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Devri.Interact;
using Windows.Storage;
using Devri;

namespace Devri.Common
{
    class Feeling
    {
        //Device Information SAVE/LOAD ETC
        static public readonly string PIN = "10032";
        static public readonly string CODE = "RLAWN";

        static public int XAxis=0;
        static public int YAxis=0;
        static public int Status = 0; // 0 = Normal 1=Positive 2=Angry 3=Sad 
        public static List<String> feel_table = new List<string> { {"Normal"},{"Positive"},{"Angry"},{"Sad"} };
        public DateTime LastestTime,EndTime;

        

        public static void SetXAxis(int distance)
        {
            XAxis = XAxis + distance;
            GetStatus();
            MainPage.Change_Image(Status,false);
             SaveFeelAsync();
        }
        public static  void  SetYAxis(int distance)
        {
            YAxis = YAxis + distance;
            GetStatus();
            MainPage.Change_Image(Status, false);
            SaveFeelAsync();
        }
        public int GetXAxis()
        {
            return XAxis;
        }
        public int GetYAxis()
        {
            return YAxis;
        }


        static public int GetStatus()
        {
            
            return Status;
        }

        public static void InitializeFeel()
        {
            LoadFeel();
            Check_Status();
            FileStream fs = File.Create("save.txt");
        }
        public static void Check_Status()
        {
            if ((Math.Abs(XAxis) <= 20 && Math.Abs(YAxis) <= 20) || ((XAxis >= 0) && (YAxis < 0)))
            {
                Status = 0;
            }
            else if (XAxis >= 0)
            {
                if (YAxis >= 0)
                    Status = 1;
                else
                    Status = 0;
            }
            else if (XAxis < 0)
            {
                if (YAxis >= 0)
                    Status = 2;
                else
                    Status = 3;
            }



        }
        public async static void SaveFeelAsync()
        {
            FileStream fs = new FileStream("save.txt",FileMode.Append);
            StreamWriter w = new StreamWriter(fs);

            w.WriteLine(XAxis);
            w.WriteLine(YAxis);
            await ServerCommunication.POSTAsync("http://iwin247.kr:80/device/updatestatus", "pin=" + Feeling.PIN+ "?Axis_X"+Feeling.XAxis+"?Axis_Y" + Feeling.YAxis);

        }
        public static void LoadFeel()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            FileStream fs = new FileStream(@"\\c$\User Folders \ LocalAppData \ c3d9ddda-8277-426a-9dec-f262be0768ad_1.0.0.0_arm__am7z1jhasv14j \ AppData \" + "save.txt", FileMode.Append);
            StreamReader sr = new StreamReader(fs);


            XAxis = Int32.Parse(sr.ReadLine());
            YAxis = Int32.Parse(sr.ReadLine());
        }
        public void Usersleep_Start()
        {
            LastestTime = DateTime.Now;
        }
        public void Usersleep_End()
        {
            ServerCommunication.GET("http://iwin247.kr:80/usersleep/end", "Device Code");
            EndTime = DateTime.Now;
            
        }
        public void Update_image(int stat)
        {
            MainPage.Change_Image(stat,false);
        }
        public static JObject Dday;
        public void Add_DDay() {  }
        public void Delete_DDay() { }
    }
}
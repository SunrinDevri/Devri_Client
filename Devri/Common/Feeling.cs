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

        public static async Task InitializeFeelAsync()
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("save.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            LoadFeelAsync();
            Check_Status();
           
           
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
        public static async void LoadFeelAsync()
        {
            //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Windows.Storage.StorageFile sampleFile =  await installedLocation.CreateFileAsync("save.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);

            //이거 썻었다가 안되서 포기함
            // FileStream fs = new FileStream("save.txt", FileMode.Append);
            // StreamReader sr = new StreamReader(fs);
            IList<string> loaded_feel =  await FileIO.ReadLinesAsync(sampleFile); //use this to read line 

            XAxis = Int32.Parse(loaded_feel[0]);
            YAxis = Int32.Parse(loaded_feel[1]);
        }
        public void Update_image(int stat)
        {
            MainPage.Change_Image(stat, false);
        }
        public static JObject Dday;
        public void Add_DDay(DateTime at,string name)
        {
            
            Dday.Add("Name", name);
            Dday.Add("Year", at.Year);
            Dday.Add("Name", at.Month);
            Dday.Add("Name", at.Day);

        }
        

    }
        
        
 }
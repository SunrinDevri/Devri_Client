﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Devri.Interact;

namespace Devri.Common
{
    class Feeling
    {
        public int XAxis;
        public int YAxis;
        public int Status = 0; // 0 = Normal 1=Positive 2=Angry 3=Sad 
        public DateTime LastestTime;

        public void SetXAxis(int distance)
        {
            XAxis = XAxis + distance;
        }
        public void SetYAxis(int distance)
        {
            YAxis = YAxis + distance;
        }

        Feeling()
        {
        }

        public int GetXAxis()
        { return XAxis; }
        public int GetYAxis()
        { return YAxis; }

        public void InitializeFeel()
        {
            XAxis = 0;
            YAxis = 0;
            Check_Status();
            FileStream fs = File.Create("save.txt");
        }
        public int Check_Status()
        {
            if ((Math.Abs(XAxis) <= 20 && Math.Abs(YAxis) <= 20) || ((XAxis >= 0) && (YAxis < 0)))
            {
                return 0;
            }
            else if (XAxis >= 0)
            {
                if (YAxis >= 0)
                    return 1;
                else
                    return 0;
            }
            else if (XAxis < 0)
            {
                if (YAxis >= 0)
                    return 2;
                else
                    return 3;
            }
            
            
            return -1;
        }
        public void SaveFeel()
        {
            FileStream fs = new FileStream("save.txt",FileMode.Append);
            StreamWriter w = new StreamWriter(fs);

            w.WriteLine(XAxis);
            w.WriteLine(YAxis);

        }
        public void LoadFeel()
        {
            FileStream fs = new FileStream("save.txt", FileMode.Append);
            StreamReader sr = new StreamReader(fs);


            XAxis = Int32.Parse(sr.ReadLine());
            YAxis = Int32.Parse(sr.ReadLine());
        }
        public void Usersleep_Start()
        {
            ServerCommunication.GET("127.0.0.1/usersleep/start", "Device Code");
        }
        public void Usersleep_End()
        {
            ServerCommunication.GET("127.0.0.1/usersleep/end", "Device Code");
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Devri.Common
{
    class Feeling
    {
        public static int XAxis;
        public static int YAxis;
        public int Status = 0; // 0 = Normal 1=Positive 2=Angry 3=Sad 
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

    }
}
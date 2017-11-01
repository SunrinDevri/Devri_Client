using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Devri.Common
{
    class Feeling
    {
        public int XAxis;
        public int YAxis;
        public int Status; // 0 = Normal 1=Positive 2=Angry 3=Sad 
        public void SetXAxis(int distance)
        {
        }
        public void SetYAxis(int distance)
        {
        }
        public int GetXAxis()
        { return XAxis; }
        public int GetYAxis()
        { return YAxis; }
        Feeling()
        {
        }
        public void InitializeFeel()
        {
            XAxis = 0;
            YAxis = 0;
        }

    }
}
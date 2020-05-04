using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vtolvrRandomFailures.Plugins
{
    class mfdGray :BaseFailure
    {
        
        public mfdGray()
        {

            failureName = "Gray Out MFD";
            failureDescription = "Test";
            failureCategory = "Avionics";
            failureRate = 0.9;
            failureEnabled = false;

        }
    }
}

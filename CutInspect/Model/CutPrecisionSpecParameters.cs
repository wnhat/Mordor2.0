using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutInspect.Model
{
    public class CutPrecisionSpecParameters
    {
        public int specWidth { get; private set; }
        public int specCenterShift { get; private set; }
        public CutPrecisionSpecParameters()
        {
            specWidth = 10;
            specCenterShift = 50;
        }
    }
}

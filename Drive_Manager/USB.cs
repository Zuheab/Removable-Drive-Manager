using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive_Manager
{
    class USB
    {
        public String DriveName { get; private set; }


        public USB(String Name)
        {
            this.DriveName = Name;

        }
    }
}

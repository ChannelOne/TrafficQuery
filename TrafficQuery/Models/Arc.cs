using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficQuery.Models
{
    class Arc
    {
        public uint NodeID { get; set; }
        public object UltraData { get; set; }

        private uint weight = 1;
        public uint Weight
        {
            get { return weight; }
            set { weight = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ZapMobileApi.Models
{
    public class SensorModel
    {
        public int SensorId { get; set; }
        public string SensorName { get; set; }
        public int Min_value { get; set; }
        public int Max_value { get; set; }
        public string  Unit { get; set; }
        
        public int MachineId { get; set; }
        public virtual MachineModel Machine { get; set; }

    }
}

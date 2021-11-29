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

    public class SensorList
    {
        public int sensorId { get; set; }
        public string sensorName { get; set; }
        public int value_max { get; set; }
        public int value_min { get; set; }
        public string unit { get; set; }
    }

    public class SensorInfo
    {
        public int machineId { get; set; }
        public string machineName { get; set; }
        public List<SensorList> sensorList { get; set; }
    }
    public class SensorAverage
    {
        public string SensorId { get; set; }
        public long  AverageValue { get; set; }
        public int Valid { get; set; }
        public int Invalid { get; set; }

    }    
}

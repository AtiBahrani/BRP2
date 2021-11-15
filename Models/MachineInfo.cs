using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZapMobileApi.Models
{
    
    public class Location
    {
        public int locationId { get; set; }
        public string locationName { get; set; }
    }
     
    public class Sensor
    {
        public string type { get; set;}
        public long value { get; set; }
        public string unit { get; set; }
    }

    public class Measure
    {
        public DateTime timestamp { get; set; }
        public List<Sensor> sensorList { get; set; }

    }
    public class MachineInfo
    {
        public int machineId { get; set; }
        public string machineName { get; set; }
        public bool status { get; set; }
        public int locationId { get; set; }
        public Location location { get; set; }
        public Measure measure { get; set; }
    }



}

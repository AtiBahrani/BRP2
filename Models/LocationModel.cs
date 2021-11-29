using System;
using System.Collections.Generic;
using System.Text;

namespace ZapMobileApi.Models
{
    public class LocationModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }

    }
    public class Notification
    {
        public int machineId { get; set; }
        public string machineName { get; set; }
        public string sensorName { get; set; }
        public long sensorValue { get; set; }
        public string type { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class UserNotification
    {
        public string userName { get; set; }
        public Location location { get; set; }
        public List<Notification> notifications { get; set; }
    }

}

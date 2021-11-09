using System;
using System.Collections.Generic;
using System.Text;

namespace ZapMobileApi.Models
{
    public class MeasurementsModel
    {
        public int MeasurementId { get; set; }
        public long  Value { get; set; }
        public DateTime Timestamp { get; set; }
        public int SensorId { get; set; }
        public virtual SensorModel Sensor { get; set; }

    }
}

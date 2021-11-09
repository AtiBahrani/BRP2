using System;
using System.Collections.Generic;
using System.Text;

namespace ZapMobileApi.Models
{
    public class MachineModel
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public bool Status { get; set; }
        public int LocationId { get; set; }
        public virtual LocationModel Location { get; set; }

    }
}

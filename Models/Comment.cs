using System;
using System.Collections.Generic;
using System.Text;

namespace ZapMobileApi.Models
{
    public class Comment
    {
        public string UserName { get; set; }
        public int MachineId { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }

    }

    public class MachineComment
    {
        public string message { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class CommentInfo
    {
        public int machineId { get; set; }
        public List<MachineComment> comments { get; set; }
    }

}

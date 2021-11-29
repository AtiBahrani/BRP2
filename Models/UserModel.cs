using System;
using System.Collections.Generic;
using System.Text;

namespace ZapMobileApi.Models
{
    public class UserModel
    {
        public int UserId { get; set; } 
        public string Email { get; set; }
        public string Location { get; set; }
        public int LocationId { get; set; }
    }

    public class UserInfo
    {
        public string userName { get; set; }
        public Location location { get; set; }
    }
}

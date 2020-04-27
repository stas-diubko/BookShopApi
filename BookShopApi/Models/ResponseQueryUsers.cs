using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApi.Models
{
    public class ResponseQueryUsers
    {
        public List<User> data { get; set; }

        public long usersLength { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApi.Models
{
    public class ResponseGeneralWithToken
    {
        public bool success { get; set; }

        public string message { get; set; }

        public JWTAuthResponse data { get; set; }
    }
}

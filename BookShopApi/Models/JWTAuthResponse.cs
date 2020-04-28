using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApi.Models
{
    public class JWTAuthResponse
    {
        public string token { get; set; }

        public string refreshToken { get; set; }
    }
}

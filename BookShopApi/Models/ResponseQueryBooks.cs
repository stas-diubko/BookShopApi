using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApi.Models
{
    public class ResponseQueryBooks
    {
        public List<Book> data { get; set; }

        public long booksLength { get; set; }
    }
}

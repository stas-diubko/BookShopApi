using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShopApi.Models
{
    public class PasswordUpdate
    {
        string currentPassword { get; set; }
        string newPassword { get; set; }
        string confirmedNewPassword { get; set; }
}
}

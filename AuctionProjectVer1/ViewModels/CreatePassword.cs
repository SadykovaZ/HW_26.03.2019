using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionProjectVer1.ViewModels
{
    public class CreatePassword
    {
        public string Email { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string newPasswordConfirmation { get; set; }

        public string ApplicationUserId { get; set; }        
        public string PasswordHash { get; set; }
    }
}

using AuctionProjectVer1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuctionProjectVer1
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //AccountService s = new AccountService();
            //s.OpenOrganization(new ViewModels.OpenOrganizationViewModel()
            //{
            //    OrganizationIdentificationNumber = "2589645",
            //    OrganizationFullName = "АО 'КТГ'",
            //    OrganizationTypeId = 1,
            //    CeoFirstName = "R",
            //    CeoLastName = "L",
            //    CeoMiddleName = "M",
            //    Email = "sss@mail.ru",
            //    DoB = new DateTime(1986, 05, 12)
            //});

            PasswordService ps = new PasswordService();
            //ps.CreatePassword(new ViewModels.CreatePassword()
            //{
               
            //    PasswordHash="fgp125"
            //});

            ps.ChangePassword(2, "213rtyuu");
            //s.GetGeolocationInfo();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}

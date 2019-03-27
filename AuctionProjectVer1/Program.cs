using AuctionProjectVer1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuctionProjectVer1
{
    static class Program
    {
        public static string CreateMD5(string input)
        {
            
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

               
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

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

            //    PasswordHash = CreateMD5("password")
            //});

           ps.ChangePassword(1, CreateMD5("jkl"));
            //s.GetGeolocationInfo();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}

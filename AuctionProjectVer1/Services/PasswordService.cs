using AuctionProjectVer1.Extensions;
using AuctionProjectVer1.Models;
using AuctionProjectVer1.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuctionProjectVer1.Services
{
    public class PasswordService
    {
        public void CreatePassword(CreatePassword password)
        {
            DataSet applicationPasswordDataSet = new DataSet();
            string userPassword = "[dbo].[ApplicationUserPasswordHistories]";

            using (SqlConnection applicationPasswordConnection= new SqlConnection(ApplicationSettings.IDENTITY_CONNECTION_STRING))
            {
                applicationPasswordConnection.Open();
                string selectUserPassword = $"select * from {userPassword}";

                using (SqlDataAdapter passwordAdapter = new SqlDataAdapter(selectUserPassword, applicationPasswordConnection))
                {
                    passwordAdapter.Fill(applicationPasswordDataSet, userPassword);
                    SqlCommandBuilder userPasswordCommandBuilder = new SqlCommandBuilder(passwordAdapter);
                    ApplicationUserPasswordHisroty uPassword = new ApplicationUserPasswordHisroty()
                    {
                        ApplicationUserId = Guid.NewGuid().ToString(),
                        SetupDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        InvalidatedDate = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"),
                        PasswordHash = password.PasswordHash
                    };

                    passwordAdapter.SelectCommand = new SqlCommand(selectUserPassword, applicationPasswordConnection);
                    userPasswordCommandBuilder = new SqlCommandBuilder(passwordAdapter);

                    passwordAdapter.Fill(applicationPasswordDataSet, userPassword);
                    var dataRow = applicationPasswordDataSet.Tables[userPassword].NewRowWithData(uPassword);
                    applicationPasswordDataSet.Tables[userPassword].Rows.Add(dataRow);
                    passwordAdapter.Update(applicationPasswordDataSet, userPassword);

                }


            }
        }

        public void ChangePassword(int id, string newPassword)
        {

            DataSet applicationPasswordDataSet = new DataSet();
            string userPassword = "[dbo].[ApplicationUserPasswordHistories]";
            string selectUserPassword = $"select * from {userPassword}";
            string UpdateUserPassword = $"update {userPassword} set [PasswordHash]={newPassword} where [Id]={id}";

            using (SqlConnection applicationPasswordConnection = new SqlConnection(ApplicationSettings.IDENTITY_CONNECTION_STRING))
            {
                using (SqlDataAdapter passwordAdapter = new SqlDataAdapter(selectUserPassword, applicationPasswordConnection))
                {
                    //applicationPasswordConnection.Open();

                    passwordAdapter.Fill(applicationPasswordDataSet, userPassword);

                    SqlCommandBuilder com = new SqlCommandBuilder(passwordAdapter);

                    passwordAdapter.SelectCommand = new SqlCommand(selectUserPassword, applicationPasswordConnection);
                    com = new SqlCommandBuilder(passwordAdapter);
                    using (SqlDataAdapter updatePassAdap = new SqlDataAdapter())
                    {
                        try
                        {
                            applicationPasswordConnection.Open();
                            SqlCommand cmd = new SqlCommand(UpdateUserPassword, applicationPasswordConnection);
                            updatePassAdap.UpdateCommand = cmd;
                            updatePassAdap.Update(applicationPasswordDataSet, userPassword);

                            SqlCommandBuilder com1 = new SqlCommandBuilder(updatePassAdap);
                            com1 = new SqlCommandBuilder(updatePassAdap);
                            //updatePassAdap.SelectCommand = new SqlCommand(selectUserPassword, applicationPasswordConnection);
                            //updatePassAdap.Fill(applicationPasswordDataSet, userPassword);
                            //updatePassAdap.Update(applicationPasswordDataSet, userPassword);


                            applicationPasswordDataSet.AcceptChanges();
                            MessageBox.Show("Password updated");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    passwordAdapter.Update(applicationPasswordDataSet, userPassword);

                }


            }
        }

    }

}

using AuctionProjectVer1.Extensions;
using AuctionProjectVer1.Models;
using AuctionProjectVer1.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace AuctionProjectVer1.Services
{
    public class PasswordService
    {
        public void CreatePassword(CreatePassword password)
        {
            DataSet applicationPasswordDataSet = new DataSet();
            string userPassword = "[dbo].[ApplicationUserPasswordHistories]";

            using (SqlConnection applicationPasswordConnection = new SqlConnection(ApplicationSettings.IDENTITY_CONNECTION_STRING))
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
                    MessageBox.Show("Пароль добавлен");

                }
            }
        }
        public void ChangeUserPassword(CreatePassword passwordViewModel)
        {
            DataSet identityDataSet = new DataSet();
            string userTable = "[dbo].[ApplicationUsers]";
            string userPasswordTable = "[dbo].[ApplicationUserPasswordHistories]";
            string userId = string.Empty;

            using (SqlConnection identityConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["IdentityDbConnectionString"].ConnectionString))
            {
                identityConnection.Open();

                string selectEmailPasswordSql = $"select u.Email, u.Id, p.SetupDate, p.PasswordHash " +
                    $"from {userTable} u, {userPasswordTable} p " +
                    $"where u.Id=p.ApplicationUserId  and u.Email='{passwordViewModel.Email}'" +
                    $"order by p.SetupDate desc";

                using (SqlDataAdapter identityAdapter = new SqlDataAdapter(selectEmailPasswordSql, identityConnection))
                {
                    
                    identityAdapter.Fill(identityDataSet);
                    SqlCommandBuilder identityCommandBuilder = new SqlCommandBuilder(identityAdapter);
                    DataTable dataTable = identityDataSet.Tables[0];
                    userId = dataTable.Rows[1][0].ToString();

                    int cnt = 0;
                    if (dataTable.Rows.Count <= 5)
                        cnt = dataTable.Rows.Count;
                    else if (dataTable.Rows.Count > 5)
                        cnt = 5;

                    for (int i = 0; i < cnt; i++)
                    {
                        if (passwordViewModel.newPassword == dataTable.Rows[i][3].ToString())
                            throw new ApplicationException($"{passwordViewModel.newPassword} уже был использован");
                    }
                    
                    
                    identityDataSet.Clear();
                    string selectUserPassword = $"select * from {userPasswordTable} " +
                        $"where ApplicationUserId = '{userId}' " +
                        $"and InvalidatedDate is null";
                    identityAdapter.SelectCommand = new SqlCommand(selectUserPassword, identityConnection);
                    identityCommandBuilder = new SqlCommandBuilder(identityAdapter);

                    identityAdapter.Fill(identityDataSet);
                    DataTable table = identityDataSet.Tables[0];
                    table.Rows[1]["InvalidatedDate"] = DateTime.Now.ToString("yyyy-MM-dd");
                    identityAdapter.Update(identityDataSet);
                    
                    identityDataSet.Clear();
                    ApplicationUserPasswordHisroty userPassword = new ApplicationUserPasswordHisroty()
                    {
                        Id = 8,
                        ApplicationUserId = userId,
                        SetupDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        PasswordHash = passwordViewModel.newPassword
                    };

                    string userPasswordSql = $"select * from {userPasswordTable}";
                    identityAdapter.SelectCommand = new SqlCommand(userPasswordSql, identityConnection);
                    identityCommandBuilder = new SqlCommandBuilder(identityAdapter);

                    identityAdapter.Fill(identityDataSet, userPasswordTable);
                    var dataRow = identityDataSet.Tables[userPasswordTable].NewRowWithData(userPassword);
                    identityDataSet.Tables[userPasswordTable].Rows.Add(dataRow);
                    identityAdapter.Update(identityDataSet, userPasswordTable);
                }
            }
        }

        //public void ChangePassword(string appUserId, string newPassword)
        //{
        //    DataSet applicationPasswordDataSet = new DataSet();
        //    string userPassword = "[dbo].[ApplicationUserPasswordHistories]";
        //    string selectUserPassword = $"select * from {userPassword}";
        //    //string UpdateUserPassword = $"update {userPassword} set [PasswordHash]={newPassword} where [Id]={id}";
        //    using (TransactionScope beginTran = new TransactionScope())
        //    {
        //        try
        //        {
        //            using (SqlConnection applicationPasswordConnection = new SqlConnection(ApplicationSettings.IDENTITY_CONNECTION_STRING))
        //            {
        //                applicationPasswordConnection.Open();

        //                using (SqlDataAdapter passwordAdapter = new SqlDataAdapter(selectUserPassword, applicationPasswordConnection))
        //                {
        //                    SqlCommandBuilder com = new SqlCommandBuilder(passwordAdapter);
        //                    passwordAdapter.Fill(applicationPasswordDataSet, userPassword);
                            
        //                    applicationPasswordDataSet.Tables[userPassword].PrimaryKey = new DataColumn[] { applicationPasswordDataSet.Tables[userPassword].Columns["Id"] };
        //                    DataRow find = applicationPasswordDataSet.Tables[userPassword].Rows.Find(2);
        //                    string valueFind = find["ApplicationUserId"].ToString();
                            

        //                    if (valueFind==appUserId)
        //                    {
        //                        string PassFind = find["PasswordHash"].ToString();                                
        //                        if (newPassword == PassFind)
        //                        {
        //                            throw new ApplicationException("Такой пароль уже был использован");
        //                        }
        //                        else
        //                        {
        //                            passwordAdapter.Fill(applicationPasswordDataSet, userPassword);
        //                            SqlCommandBuilder com2 = new SqlCommandBuilder(passwordAdapter);
        //                            ApplicationUserPasswordHisroty uPassword = new ApplicationUserPasswordHisroty()
        //                            {
        //                                Id = 7,
        //                                ApplicationUserId = appUserId,
        //                                SetupDate = DateTime.Now.ToString("yyyy-MM-dd"),
        //                                InvalidatedDate = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd"),
        //                                PasswordHash = newPassword
        //                            };
        //                            passwordAdapter.SelectCommand = new SqlCommand(selectUserPassword, applicationPasswordConnection);
        //                            com2 = new SqlCommandBuilder(passwordAdapter);
        //                            passwordAdapter.Fill(applicationPasswordDataSet, userPassword);
        //                            var dataRow = applicationPasswordDataSet.Tables[userPassword].NewRowWithData(uPassword);
        //                            applicationPasswordDataSet.Tables[userPassword].Rows.Add(dataRow);                                   
        //                            passwordAdapter.Update(applicationPasswordDataSet, userPassword);
        //                            MessageBox.Show("Новый пароль добавлен");
        //                        }
        //                    }
                            
        //                    //SqlCommandBuilder com1 = new SqlCommandBuilder(passwordAdapter);
        //                    passwordAdapter.Update(applicationPasswordDataSet, userPassword);
        //                    //MessageBox.Show("Пароль обновлен");
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
        //    }
        //}
    }
}

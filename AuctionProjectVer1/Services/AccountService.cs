using AuctionProjectVer1.Extensions;
using AuctionProjectVer1.Models;
using AuctionProjectVer1.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AuctionProjectVer1.Services
{
    public class AccountService
    {
        public void OpenOrganization(OpenOrganizationViewModel viewModel)
        {
            DataSet applicationDataSet = new DataSet();
            DataSet identityDataSet = new DataSet();

            string organizationsTable = "[dbo].[Organizations]";
            string employeeTable = "[dbo].[Employees]";

            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    using (SqlConnection applicationConnection = new SqlConnection(
                                    ApplicationSettings.APPLICATION_CONNECTION_STRING))
                    {
                        applicationConnection.Open();

                        string selectOrganizationByIdentificatorsSql = $"select * from {organizationsTable} " +
                            $"where [IdentificationNumber] = {viewModel.OrganizationIdentificationNumber}";

                        string selectOrganizationsSql = $"select * from {organizationsTable}";

                        using (SqlDataAdapter applicationAdapter = new SqlDataAdapter(
                            selectOrganizationByIdentificatorsSql,
                            applicationConnection))
                        {
                            applicationAdapter.Fill(applicationDataSet, organizationsTable);
                            SqlCommandBuilder applicationCommandBuilder = new SqlCommandBuilder(applicationAdapter);
                            bool isOrganizationAlreadyExist = applicationDataSet
                                .Tables[organizationsTable].Rows.Count != 0;

                            if (isOrganizationAlreadyExist)
                                throw new ApplicationException($"Already has an organization with IdentificationNumber = {viewModel.OrganizationIdentificationNumber}");

                            applicationDataSet.Clear();

                            Organization organization = new Organization()
                            {
                                FullName = viewModel.OrganizationFullName,
                                IdentificationNumber = viewModel.OrganizationIdentificationNumber,
                                OrganizationTypeId = viewModel.OrganizationTypeId,
                                RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd")
                            };

                            applicationAdapter.SelectCommand = new SqlCommand(selectOrganizationsSql, applicationConnection);
                            applicationCommandBuilder = new SqlCommandBuilder(applicationAdapter);

                            applicationAdapter.Fill(applicationDataSet, organizationsTable);
                            var dataRow = applicationDataSet.Tables[organizationsTable].NewRowWithData(organization);
                            applicationDataSet.Tables[organizationsTable].Rows.Add(dataRow);
                            applicationAdapter.Update(applicationDataSet, organizationsTable);

                            Employee employee = new Employee()
                            {
                                FirstName = viewModel.CeoFirstName,
                                LastName = viewModel.CeoLastName,
                                MiddleName = viewModel.CeoMiddleName,
                                Email = viewModel.Email,
                                DoB = viewModel.DoB,
                                OrganizationId = Guid.NewGuid().ToString()
                            };
                            string selectEmployeeSql = $"select * from {employeeTable}";

                            applicationAdapter.SelectCommand = new SqlCommand(selectEmployeeSql, applicationConnection);
                            applicationCommandBuilder = new SqlCommandBuilder(applicationAdapter);
                            applicationAdapter.Fill(applicationDataSet, employeeTable);

                            dataRow = applicationDataSet.Tables[employeeTable].NewRowWithData(employee);
                            applicationDataSet.Tables[employeeTable].Rows.Add(dataRow);
                            applicationAdapter.Update(applicationDataSet, employeeTable);



                            //transactionScope.Complete();
                        }
                    }
                    using (SqlConnection identityConnection = new SqlConnection(
                        ApplicationSettings.IDENTITY_CONNECTION_STRING))
                    {
                        identityConnection.Open();

                        string userTable = "[dbo].[ApplicationUsers]";
                        string selectUserSql = $"select * from {userTable}";

                        using (SqlDataAdapter identityUserAdapter = new SqlDataAdapter(selectUserSql, identityConnection))
                        {
                            identityUserAdapter.Fill(identityDataSet, userTable);
                            SqlCommandBuilder identityCommandBuilder = new SqlCommandBuilder(identityUserAdapter);

                            ApplicationUser application = new ApplicationUser()
                            {
                                Email = viewModel.Email
                            };
                            identityUserAdapter.SelectCommand = new SqlCommand(selectUserSql, identityConnection);
                            identityCommandBuilder = new SqlCommandBuilder(identityUserAdapter);

                            identityUserAdapter.Fill(identityDataSet, userTable);
                            var dataRow = identityDataSet.Tables[userTable].NewRowWithData(application);
                            identityDataSet.Tables[userTable].Rows.Add(dataRow);
                            //identityUserAdapter.Update(identityDataSet, userTable);

                            transactionScope.Complete();

                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void GetGeolocationInfo()
        {
            WebClient webClient = new WebClient();
            string externalIp = webClient
                .DownloadString("http://icanhazip.com");

            string ipStackAccessKey = "...";
            string ipStackUrl = $"api.ipstack.com/{externalIp}?access_key={ipStackAccessKey}";
            ipStackUrl = "http://" + ipStackUrl;

            string ipInfoAsJson = webClient.DownloadString(ipStackUrl);
        }
    }
}

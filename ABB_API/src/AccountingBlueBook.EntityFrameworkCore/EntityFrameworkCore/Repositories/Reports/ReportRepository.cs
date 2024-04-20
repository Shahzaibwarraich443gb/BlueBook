using Abp.Data;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using AccountingBlueBook.Entities.MainEntities.Customers.Dto;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using AccountingBlueBook.Entities.MainEntities.Reports;
using AccountingBlueBook.Entities.MainEntities.Reports.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace AccountingBlueBook.EntityFrameworkCore.Repositories.Reports
{
    public class ReportRepository : AccountingBlueBookRepositoryBase<Invoice, long>, IReportRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly IUnitOfWork _unitOfWork;

        public ReportRepository(IDbContextProvider<AccountingBlueBookDbContext> dbContextProvider,
                                 IActiveTransactionProvider transactionProvider,
                                 IUnitOfWork unitOfWork) : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DailyReceiptDto>> GetAllDailyRecepit(DateTime? startdate, DateTime? enddate, long? _PaymentMethodId, long? _AccountId, int CompanyID)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("CompanyID", CompanyID),
                new SqlParameter("startdate", startdate),
                new SqlParameter("enddate", enddate),
                new SqlParameter("PaymentMethod", _PaymentMethodId),
                new SqlParameter("AccountId", _AccountId),
                new SqlParameter("IsDeleted", 0),
            };

            using (var command = CreateCommand("sp_Finance_Report_Get_DailyReceipt", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    List<DailyReceiptDto> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new DailyReceiptDto
                        {
                            InvoiceId = dataReader["InvoiceId"] != DBNull.Value ? (long)dataReader["InvoiceId"] : null,
                            RefCustomerId = dataReader["RefCustomerId"] != DBNull.Value ? (long)dataReader["RefCustomerId"] : null,
                            CompanyId = dataReader["CompanyId"] != DBNull.Value ? (long)dataReader["CompanyId"] : null,
                            RefPaymentMethodId = dataReader["RefPaymentMethodId"] != DBNull.Value ? (int)dataReader["RefPaymentMethodId"] : null,
                            RefDepositToAccountId = dataReader["RefDepositToAccountId"] != DBNull.Value ? (long)dataReader["RefDepositToAccountId"] : null,
                            Total = dataReader["Total"] != DBNull.Value ? (decimal)dataReader["Total"] : null,
                            OpenBalance = dataReader["OpenBalance"] != DBNull.Value ? (decimal)dataReader["OpenBalance"] : null,
                            PaidAmount = dataReader["PaidAmount"] != DBNull.Value ? (decimal)dataReader["PaidAmount"] : null,
                            PaymentDate = dataReader["PaymentDate"] != DBNull.Value ? (DateTime)dataReader["PaymentDate"] : null,
                            InvoiceNo = dataReader["InvoiceNo"].ToString(),
                            PaymentMethod = dataReader["PaymentMethod"].ToString(),
                            Company = dataReader["Company"].ToString(),
                            CSR = dataReader["CSR"].ToString(),
                            EmployeeName = dataReader["EmployeeName"].ToString(),
                            CustomerName = dataReader["CustomerName"].ToString(),
                            AccountDescription = dataReader["AccountDescription"].ToString(),
                        };
                        transList.Add(transObj);
                    }

                    return transList;
                }
            }
        }
        public async Task<List<AuditLogsDto>> GetAllAuditlogs(DateTime startdate, DateTime enddate, int tenatid,string MethodName)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("@TenantId", tenatid),
                new SqlParameter("StartDate", startdate),
                new SqlParameter("EndDate", enddate),
                new SqlParameter("EmployeeId", 0),
                new SqlParameter("@Method",MethodName)

            };

            using (var command = CreateCommand("get_AuditLogs_report", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    List<AuditLogsDto> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new AuditLogsDto
                        {


                            
                            CustomerName = dataReader["Name"].ToString(),
                          CustomerNo   = (int)dataReader["CompanyId"],
                            CompanyName = dataReader["CompanyName"].ToString(),
                            AppLog = dataReader["ServiceName"].ToString(),
                            LogDate = (DateTime)dataReader["ExecutionTime"],
                            //OperationByUserId = (long)dataReader["OperationByUserId"],
                            UserName = dataReader["OperationByName"].ToString(),
                            Operation = dataReader["MethodName"].ToString(),

                            //UserType = "Customer"


                        };
                        transList.Add(transObj);
                    }

                    return transList;
                }
            }
        }

        public async Task<List<BalanceSheetDto>> GetBalanceSheet(DateTime startDate, DateTime endDate, int tenantId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("tenantId", tenantId),
                new SqlParameter("startDate", startDate),
                new SqlParameter("endDate", endDate),
                new SqlParameter("IsDeleted", 0),
            };

            using (var command = CreateCommand("sp_Report_Get_BalanceSheet", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    List<BalanceSheetDto> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new BalanceSheetDto();
                        {

                        };
                        transList.Add(transObj);
                    }

                    return transList;
                }
            }
        }



        private DbCommand CreateCommand(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            var dbContext = GetDbContextAsync().Result;
            var command = dbContext.Database.GetDbConnection().CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = GetActiveTransaction();

            command.CommandTimeout = 180;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
        private async Task EnsureConnectionOpenAsync()
        {
            var connection = (await GetDbContextAsync()).Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }
        }
        private DbTransaction GetActiveTransaction()
        {
            return (DbTransaction)_transactionProvider.GetActiveTransaction(new ActiveTransactionProviderArgs
            {
                {"ContextType", typeof(AccountingBlueBookDbContext) },
                {"MultiTenancySide", MultiTenancySide }
            });
        }

        //public Task<List<SourceReferalDto>> GetAllSourceReferal(DateTime startDate, DateTime endDate, long SoureceReferalId)
        //{

        //}
        public async Task<List<SourceReferalDto>> GetAllSourceReferal(DateTime startdate, DateTime enddate,  long SoureceReferalId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                
                new SqlParameter("StartDate", startdate),
                new SqlParameter("EndDate", enddate),
               
                new SqlParameter("@SourceReferalId",SoureceReferalId)

            };

            using (var command = CreateCommand("GetTransactionsBySourceReferal", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    List<SourceReferalDto> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new SourceReferalDto
                        {



                            //CustomerName = dataReader["Name"].ToString(),
                            //CustomerNo = (int)dataReader["CompanyId"],
                            //CompanyName = dataReader["CompanyName"].ToString(),
                            //AppLog = dataReader["ServiceName"].ToString(),
                            //LogDate = (DateTime)dataReader["ExecutionTime"],
                            ////OperationByUserId = (long)dataReader["OperationByUserId"],
                            //UserName = dataReader["OperationByName"].ToString(),
                            //Operation = dataReader["MethodName"].ToString(),

                            ////UserType = "Customer"
                            ///Id = (int)dataReader["Id"],
                           // CreditAmount = (decimal)dataReader["CreditAmount"],
                            CustomerName = dataReader["CustomerName"].ToString(),
                            //SourceReferralTypeId = (int)dataReader["SourceReferralTypeId"],
                            //CustomerId = (int)dataReader["CustomerId"],
                            TransactionDate = (DateTime)dataReader["TransactionDate"],
                            CreditAmount = (decimal)dataReader["CreditAmount"],


                        };
                        transList.Add(transObj);
                    }

                    return transList;
                }
            }
        }

    }
}

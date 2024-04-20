using Abp.Data;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using AccountingBlueBook.Entities.MainEntities.Invoices;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Abp.Application.Services.Dto;
using AccountingBlueBook.Entities;
using AccountingBlueBook.Entities.NormalizeEntities;
using AccountingBlueBook.AppServices.ReceivedPayment.Dto;
using AccountingBlueBook.Entities.MainEntities.Customers.Dto;
using AccountingBlueBook.AppServices.CreditNote.Dto;
using AccountingBlueBook.AppServices.Invoices.dto;
using AccountingBlueBook.Entities.MainEntities.Vouchers;
using AccountingBlueBook.AppServices.JournalVoucher.Dto;

namespace AccountingBlueBook.EntityFrameworkCore.Repositories.Invoices
{
    public class InvoiceRepository : AccountingBlueBookRepositoryBase<Invoice, long>, IInvoiceRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceRepository(IDbContextProvider<AccountingBlueBookDbContext> dbContextProvider,
                                 IActiveTransactionProvider transactionProvider,
                                 IUnitOfWork unitOfWork) : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
            _unitOfWork = unitOfWork;
        }


        public async Task<string> GetVoucherNumber(string voucherTypeCode, int tenantId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("VoucherTypeCode", voucherTypeCode),
                new SqlParameter("TenantId", tenantId),
            };

            using (var command = CreateCommand("Finance_SpGetVouvherNO", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    string voucherNumber = string.Empty;
                    while (dataReader.Read())
                    {
                        voucherNumber = dataReader["VoucherNo"].ToString();
                    }
                    return voucherNumber;
                }
            }
        }


        public async Task<string> GetInvoiceBalance(long id)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("InvoiceId", id)
            };

            using (var command = CreateCommand("sp_Finance_Get_InvoiceBalance", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    decimal InvoiceBalance = 0;
                    string Status = "Successful";
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("InvoiceBalance")))
                        {
                            InvoiceBalance = Convert.ToDecimal(dataReader["InvoiceBalance"]);
                        }
                        else
                        {
                            Status = "Failure";
                        }
                    }

                    return JsonConvert.SerializeObject(new { InvoiceBalance, Status });
                }
            }
        }

        public async Task<List<CustomerTransactionDto>> GetCustomerTransaction(int customerId, int tenantId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("custId", customerId),
                new SqlParameter("tenantId", tenantId),
            };

            using (var command = CreateCommand("sp_Customer_GetTransactions", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    List<CustomerTransactionDto> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new CustomerTransactionDto
                        {
                            Id = Convert.ToInt32(dataReader["InvoiceId"]),
                            InvoiceDate = dataReader["InvoiceDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["InvoiceDate"]) : null,
                            CreationTime = dataReader["CreationTime"] != DBNull.Value ? Convert.ToDateTime(dataReader["CreationTime"]) : null,
                            LastModificationTime = dataReader["LastModificationTime"] != DBNull.Value ? Convert.ToDateTime(dataReader["LastModificationTime"]) : null,
                            Type = dataReader["Type"].ToString(),
                            InvoiceCode = dataReader["InvoiceCode"] != null ? dataReader["InvoiceCode"].ToString() : null,
                            Product = dataReader["Product"].ToString(),
                            Description = dataReader["Description"].ToString(),
                            Csr = dataReader["CSR"].ToString(),
                            Balance = dataReader["Balance"] != DBNull.Value ? Convert.ToDecimal(dataReader["Balance"]) : null,
                            Total = dataReader["Total"] != DBNull.Value ? Convert.ToDecimal(dataReader["Total"]) : null,
                            RefCustomerId = dataReader["RefCustomerId"] != DBNull.Value ? Convert.ToInt32(dataReader["RefCustomerId"]) : 0,
                            LastModifierUserId = dataReader["LastModifierUserId"] != DBNull.Value ? Convert.ToInt32(dataReader["LastModifierUserId"]) : 0,
                            Status = dataReader["Status"].ToString(),
                            AddedBy = dataReader["AddedBy"].ToString(),
                            Company = dataReader["Company"].ToString(),
                            OrignalInvoiceNo = dataReader["OrignalInvoiceNo"].ToString(),
                            RefrenceNo = dataReader["RefrenceNo"].ToString(),
                            Email = dataReader["Email"].ToString(),
                            Note = dataReader["Note"].ToString(),
                            ComAddress = dataReader["ComAddress"].ToString(),
                            ComCity = dataReader["ComCity"].ToString(),
                            ComState = dataReader["ComState"].ToString(),
                            ComPostCode = dataReader["ComPostCode"].ToString(),
                            ComCountry = dataReader["ComCountry"].ToString(),
                            ComEmail = dataReader["ComEmail"].ToString(),
                            ComPhone = dataReader["ComPhone"].ToString(),
                            PaymentDate = dataReader["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["PaymentDate"]) : null,
                            InvoiceDueDate = dataReader["InvoiceDueDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["InvoiceDueDate"]) : null,
                        };
                        transList.Add(transObj);
                    }

                    return transList;
                }
            }
        }

        public async Task<List<CustomerTransactionDto>> GetAllTransactions(int tenantId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("tenantId", tenantId),
            };

            using (var command = CreateCommand("sp_Get_All_Transactions", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    List<CustomerTransactionDto> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new CustomerTransactionDto
                        {
                            Id = Convert.ToInt32(dataReader["InvoiceId"]),
                            InvoiceDate = dataReader["InvoiceDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["InvoiceDate"]) : null,
                            CreationTime = dataReader["CreationTime"] != DBNull.Value ? Convert.ToDateTime(dataReader["CreationTime"]) : null,
                            LastModificationTime = dataReader["LastModificationTime"] != DBNull.Value ? Convert.ToDateTime(dataReader["LastModificationTime"]) : null,
                            Type = dataReader["Type"].ToString(),
                            InvoiceCode = dataReader["InvoiceCode"] != null ? dataReader["InvoiceCode"].ToString() : null,
                            Product = dataReader["Product"].ToString(),
                            Description = dataReader["Description"].ToString(),
                            Csr = dataReader["CSR"].ToString(),
                            Balance = dataReader["Balance"] != DBNull.Value ? Convert.ToDecimal(dataReader["Balance"]) : null,
                            Total = dataReader["Total"] != DBNull.Value ? Convert.ToDecimal(dataReader["Total"]) : null,
                            RefCustomerId = dataReader["RefCustomerId"] != DBNull.Value ? Convert.ToInt32(dataReader["RefCustomerId"]) : 0,
                            LastModifierUserId = dataReader["LastModifierUserId"] != DBNull.Value ? Convert.ToInt32(dataReader["LastModifierUserId"]) : 0,
                            Status = dataReader["Status"].ToString(),
                            AddedBy = dataReader["AddedBy"].ToString(),
                            Note = dataReader["Note"].ToString(),
                            Email = dataReader["Email"].ToString(),
                            OrignalInvoiceNo = dataReader["OrignalInvoiceNo"].ToString(),
                            Company = dataReader["Company"].ToString(),
                            RefrenceNo = dataReader["RefrenceNo"].ToString(),
                            ComAddress = dataReader["ComAddress"].ToString(),
                            ComCity = dataReader["ComCity"].ToString(),
                            ComState = dataReader["ComState"].ToString(),
                            ComPostCode = dataReader["ComPostCode"].ToString(),
                            ComCountry = dataReader["ComCountry"].ToString(),
                            ComEmail = dataReader["ComEmail"].ToString(),
                            ComPhone = dataReader["ComPhone"].ToString(),
                            PaymentDate = dataReader["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["PaymentDate"]) : null,
                            InvoiceDueDate = dataReader["InvoiceDueDate"] != DBNull.Value ? Convert.ToDateTime(dataReader["InvoiceDueDate"]) : null,

                        };
                        transList.Add(transObj);
                    }

                    return transList;
                }
            }
        }

        public async Task<List<VoucherList>> GetAllVouchers(int tenantId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("tenantId", tenantId),
            };

            using (var command = CreateCommand("sp_Get_All_Vouchers", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    List<VoucherList> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new VoucherList
                        {
                            Id = (long)(dataReader["Id"] != DBNull.Value ? (long?)dataReader["Id"] : null),
                            Date = dataReader["Date"] != DBNull.Value ? Convert.ToDateTime(dataReader["Date"]) : null,
                            CreationTime = dataReader["CreationTime"] != DBNull.Value ? Convert.ToDateTime(dataReader["CreationTime"]) : null,
                            LastModificationTime = dataReader["LastModificationTime"] != DBNull.Value ? Convert.ToDateTime(dataReader["LastModificationTime"]) : null,
                           // AddedBy = dataReader["AddedBy"] != DBNull.Value ? (long?)dataReader["AddedBy"] : null,
                            //LastModifierUserId = dataReader["LastModifierUserId"] != DBNull.Value ? (long?)dataReader["LastModifierUserId"] : null,
                            VoucherNo = dataReader["VoucherNo"].ToString(),
                            VoucherTypeCode = dataReader["VoucherTypeCode"].ToString(),
                            InvoiceId = dataReader["InvoiceId"] != DBNull.Value ? (long?)dataReader["InvoiceId"] : null,
                            Company = dataReader["Company"].ToString(),
                            AddedBy = dataReader["AddedBy"].ToString(),
                            LastModifierUserId = dataReader["LastModifierUserId"].ToString(),
                            SrNo = dataReader["SrNo"].ToString(),
                            Cr_Amount = dataReader["Cr_Amount"] != DBNull.Value ? Convert.ToDecimal(dataReader["Cr_Amount"]) : null,
                            Dr_Amount = dataReader["Dr_Amount"] != DBNull.Value ? Convert.ToDecimal(dataReader["Dr_Amount"]) : null,
                            AccountName = dataReader["AccountName"].ToString(),
                        };
                        transList.Add(transObj);
                    }

                    return transList;
                }
            }
        }

        public async Task<string> GetCustomerBalance(long customerId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("CustomerId", customerId)
            };

            using (var command = CreateCommand("sp_Finance_Report_GetcustomerBalance", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    decimal CustomerBalance = 0;
                    string Status = "Successful";
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("CustomerBalanceAmount")))
                        {
                            CustomerBalance = Convert.ToDecimal(dataReader["CustomerBalanceAmount"]);
                        }
                        else
                        {
                            Status = "Failure";
                        }
                    }

                    return JsonConvert.SerializeObject(new { CustomerBalance, Status });
                }
            }
        }

        public async Task<string> GetInvoiceNo(int tenantId, int invoiceType)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("@InvoiceType", invoiceType),
                new SqlParameter("@TenantId", tenantId)
            };

            using (var command = CreateCommand("Finance_SpGetInvoiceNO", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    string invoiceNum = string.Empty;
                    string status = "Successful";
                    if (await dataReader.ReadAsync())
                    {
                        if (!dataReader.IsDBNull(dataReader.GetOrdinal("InvoiceNum")))
                        {
                            invoiceNum = dataReader["InvoiceNum"].ToString();
                        }
                        else
                        {
                            status = "Failure";
                        }
                    }

                    var result = new { InvoiceNum = invoiceNum, Status = status };

                    await dataReader.CloseAsync(); // Close the DataReader

                    return JsonConvert.SerializeObject(result);
                }
            }
        }

        public async Task<List<ReceviedPayment>> GetReceivedPaymentList(int tenantId, EntityDto input)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("CustomerID", input.Id),
                new SqlParameter("tenantId", tenantId),
                new SqlParameter("IsPaid", false)
            };

            using (var command = CreateCommand("sp_Finance_Get_CustomerUnPaidInvoices", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    string Status = "Successful";
                    List<ReceviedPayment> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new ReceviedPayment
                        {
                            InvoiceID = dataReader["InvoiceID"] != DBNull.Value ? (long?)dataReader["InvoiceID"] : null,
                            InvoiceNo = dataReader["InvoiceNo"] != DBNull.Value ? (string)dataReader["InvoiceNo"] : null,
                            InvoiceDueDate = dataReader["InvoiceDueDate"] != DBNull.Value ? (DateTime)dataReader["InvoiceDueDate"] : null,
                            OrigionalAmount = dataReader["OrigionalAmount"] != DBNull.Value ? (decimal)dataReader["OrigionalAmount"] : null,
                            OpenBalance = dataReader["OpenBalance"] != DBNull.Value ? (decimal?)dataReader["OpenBalance"] : null,
                            Product = dataReader["Product"].ToString(),
                           // ProducIDs = dataReader["ProducIDs"] != DBNull.Value ? (long?)dataReader["ProducIDs"] : null,
                            ProducIDs = dataReader["ProducIDs"].ToString(),
                            Description = dataReader["Description"].ToString(),
                        };

                        transList.Add(transObj);
                    }

                    return transList;
                }

            }
        }

        public async Task<List<ReceviedPayment>> GetPurchasePaymentList(int tenantId, EntityDto input)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("VendorId", input.Id),
                new SqlParameter("tenantId", tenantId),
                new SqlParameter("IsPaid", false)
            };

            using (var command = CreateCommand("sp_Get_vendorInvoices", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    string Status = "Successful";
                    List<ReceviedPayment> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new ReceviedPayment
                        {
                            InvoiceID = dataReader["InvoiceID"] != DBNull.Value ? (long?)dataReader["InvoiceID"] : null,
                            InvoiceNo = dataReader["InvoiceNo"] != DBNull.Value ? (string)dataReader["InvoiceNo"] : null,
                            InvoiceDueDate = dataReader["InvoiceDueDate"] != DBNull.Value ? (DateTime)dataReader["InvoiceDueDate"] : null,
                            OrigionalAmount = dataReader["OrigionalAmount"] != DBNull.Value ? (decimal)dataReader["OrigionalAmount"] : null,
                            OpenBalance = dataReader["OpenBalance"] != DBNull.Value ? (decimal?)dataReader["OpenBalance"] : null,
                            Product = dataReader["Product"].ToString(),
                            ProducIDs = dataReader["ProducIDs"].ToString(),
                            Description = dataReader["Description"].ToString(),
                        };

                        transList.Add(transObj);
                    }

                    return transList;
                }

            }
        }

        public async Task<List<ReceviedPayment>> GetReceivedPaymentDetails(int tenantId, EntityDto input)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("invoiceId", input.Id),
                new SqlParameter("tenantId", tenantId),
                //new SqlParameter("IsPaid", false)
            };

            using (var command = CreateCommand("sp_GetInvoices_from_RP", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    string Status = "Successful";
                    List<ReceviedPayment> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new ReceviedPayment
                        {
                            InvoiceID = dataReader["InvoiceID"] != DBNull.Value ? (long)dataReader["InvoiceID"] : null,
                            RefProducID = dataReader["RefProducID"] != DBNull.Value ? (long)dataReader["RefProducID"] : null,
                            InvoiceDetailId = dataReader["InvoiceDetailId"] != DBNull.Value ? (long)dataReader["InvoiceDetailId"] : null,
                            RefPaidInvoiceId = dataReader["RefPaidInvoiceId"] != DBNull.Value ? (long)dataReader["RefPaidInvoiceId"] : null,
                            RefCustomerID = dataReader["RefCustomerID"] != DBNull.Value ? Convert.ToInt32(dataReader["RefCustomerID"]) : 0,
                            RefCompanyID = dataReader["RefCompanyID"] != DBNull.Value ? Convert.ToInt32(dataReader["RefCompanyID"]) : 0,
                            RefPaymentMethodID = dataReader["RefPaymentMethodID"] != DBNull.Value ? (int)dataReader["RefPaymentMethodID"] : null,
                            RefDepositToAccountID = dataReader["RefDepositToAccountID"] != DBNull.Value ? (long)dataReader["RefDepositToAccountID"] : null,
                            InvoiceNo = dataReader["InvoiceNo"] != DBNull.Value ? (string)dataReader["InvoiceNo"] : null,
                            InvoiceDueDate = dataReader["InvoiceDueDate"] != DBNull.Value ? (DateTime)dataReader["InvoiceDueDate"] : null,
                            PaymentDate = dataReader["PaymentDate"] != DBNull.Value ? (DateTime)dataReader["PaymentDate"] : null,
                            OrigionalAmount = dataReader["OrigionalAmount"] != DBNull.Value ? (decimal)dataReader["OrigionalAmount"] : null,
                            OpenBalance = dataReader["OpenBalance"] != DBNull.Value ? (decimal)dataReader["OpenBalance"] : null,
                            Description = dataReader["Description"].ToString(),
                            RP_Invoice = dataReader["RP_Invoice"].ToString(),
                            Note = dataReader["Note"].ToString(),
                            RefrenceNo = dataReader["RefrenceNo"].ToString(),
                            PaidAmount = dataReader["PaidAmount"] != DBNull.Value ? (decimal)dataReader["PaidAmount"] : null,
                            IsSendLater = dataReader["IsSendLater"] != DBNull.Value ? (bool)dataReader["IsSendLater"] : null,
                            IsPaid = dataReader["IsPaid"] != DBNull.Value ? (bool)dataReader["IsPaid"] : null,
                        };

                        transList.Add(transObj);
                    }

                    return transList;
                }

            }
        }

        public async Task<List<invoiceDetails>> GetInvoiceDetails(int tenantId, EntityDto input)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("invoiceId", input.Id),
                new SqlParameter("tenantId", tenantId),
            };

            using (var command = CreateCommand("sp_GetInvoiceDetails", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    string Status = "Successful";
                    List<invoiceDetails> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new invoiceDetails
                        {
                            InvoiceId = dataReader["InvoiceId"] != DBNull.Value ? (long)dataReader["InvoiceId"] : null,
                            Note = dataReader["Note"] != DBNull.Value ? (string)dataReader["Note"] : null,
                            InvoiceNo = dataReader["InvoiceNo"] != DBNull.Value ? (string)dataReader["InvoiceNo"] : null,
                            InvoiceDetailId = dataReader["InvoiceDetailId"] != DBNull.Value ? (long)dataReader["InvoiceDetailId"] : null,
                            ProductId = dataReader["ProductId"] != DBNull.Value ? (int)dataReader["ProductId"] : null,
                            Quantity = dataReader["Quantity"] != DBNull.Value ? (long)dataReader["Quantity"] : null,
                            Amount = dataReader["Amount"] != DBNull.Value ? (long)dataReader["Amount"] : null,
                            IsSendLater = dataReader["IsSendLater"] != DBNull.Value ? (bool)dataReader["IsSendLater"] : null,
                            IsPaid = dataReader["IsPaid"] != DBNull.Value ? (bool)dataReader["IsPaid"] : null,
                            Rate = dataReader["Rate"] != DBNull.Value ? (long)dataReader["Rate"] : null,
                            SaleTax = dataReader["SaleTax"] != DBNull.Value ? (long)dataReader["SaleTax"] : null,
                            Discount = dataReader["Discount"] != DBNull.Value ? (long)dataReader["Discount"] : null,
                            RefTermId = dataReader["RefTermId"] != DBNull.Value ? (int)dataReader["RefTermId"] : null,
                            InvoiceDueDate = dataReader["InvoiceDueDate"] != DBNull.Value ? (DateTime)dataReader["InvoiceDueDate"] : null,
                            CreationTime = dataReader["CreationTime"] != DBNull.Value ? (DateTime)dataReader["CreationTime"] : null,
                            InvoiceDate = dataReader["InvoiceDate"] != DBNull.Value ? (DateTime)dataReader["InvoiceDate"] : null,
                            Product = dataReader["Product"].ToString(),
                            Description = dataReader["Description"].ToString(),
                            RefChartOfAccountId = dataReader["RefChartOfAccountId"] != DBNull.Value ? Convert.ToInt32(dataReader["RefChartOfAccountId"]) : 0,
                            RefCustomerId = dataReader["RefCustomerId"] != DBNull.Value ? Convert.ToInt32(dataReader["RefCustomerId"]) : 0,
                            PaidAmount = dataReader["PaidAmount"] != DBNull.Value ? Convert.ToInt32(dataReader["PaidAmount"]) : 0,
                            VendorId = dataReader["VendorId"] != DBNull.Value ? Convert.ToInt32(dataReader["VendorId"]) : 0,
                            CreditNoteDate = dataReader["CreditNoteDate"] != DBNull.Value ? (DateTime)dataReader["CreditNoteDate"] : null,
                        };

                        transList.Add(transObj);
                    }

                    return transList;
                }

            }
        }

        public async Task<List<PrintDetail>> GetPrintDetails(int tenantId, long invoiceId)
        {
            await EnsureConnectionOpenAsync();

            var sqlParams = new List<SqlParameter>()
            {
                new SqlParameter("invoiceId", invoiceId),
                new SqlParameter("tenantId", tenantId),
            };

            using (var command = CreateCommand("sp_GetPrintDetails", CommandType.StoredProcedure, sqlParams.ToArray()))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    string Status = "Successful";
                    List<PrintDetail> transList = new();

                    while (dataReader.Read())
                    {
                        var transObj = new PrintDetail
                        {
                            InvoiceId = dataReader["InvoiceId"] != DBNull.Value ? (long)dataReader["InvoiceId"] : null,
                            InvoiceDetailId = dataReader["InvoiceDetailId"] != DBNull.Value ? (long)dataReader["InvoiceDetailId"] : null,
                            ProductId = dataReader["ProductId"] != DBNull.Value ? (int)dataReader["ProductId"] : null,
                            Quantity = dataReader["Quantity"] != DBNull.Value ? (long)dataReader["Quantity"] : null,
                            IsPaid = dataReader["IsPaid"] != DBNull.Value ? (bool)dataReader["IsPaid"] : null,
                            Rate = dataReader["Rate"] != DBNull.Value ? (long)dataReader["Rate"] : null,
                            SaleTax = dataReader["SaleTax"] != DBNull.Value ? (long)dataReader["SaleTax"] : null,
                            Discount = dataReader["Discount"] != DBNull.Value ? (long)dataReader["Discount"] : null,
                            InvoiceDueDate = dataReader["InvoiceDueDate"] != DBNull.Value ? (DateTime)dataReader["InvoiceDueDate"] : null,
                            Product = dataReader["Product"].ToString(),
                            Description = dataReader["Description"].ToString(),
                            CSR = dataReader["CSR"].ToString(),
                            Amount = dataReader["PaidAmount"] != DBNull.Value ? Convert.ToInt32(dataReader["PaidAmount"]) : 0,
                            //CreditNoteDate = dataReader["CreditNoteDate"] != DBNull.Value ? (DateTime)dataReader["CreditNoteDate"] : null,
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

    }
}

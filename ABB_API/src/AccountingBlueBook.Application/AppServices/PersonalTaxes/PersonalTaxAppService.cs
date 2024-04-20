using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using AccountingBlueBook.Entities;
using AccountingBlueBook.Entities.Main;
using AccountingBlueBook.Entities.MainEntities;
using AccountingBlueBook.Entities.MainEntities.Customers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using VisioForge.Libs.ZXing;

namespace AccountingBlueBook.AppServices.PersonalTaxes
{
    public class PersonalTaxAppService : AccountingBlueBookAppServiceBase, IPersonalTaxAppService
    {
        private readonly IRepository<PersonalTax, long> personalTaxRepository;
        private readonly IRepository<Spouse, long> spouseRepository;
        private readonly IRepository<Dependent> dependentRepository;
        private readonly IRepository<IncomeDetails, long> incomeDetailRepository;
        private readonly IRepository<Customer> customerRepository;

        public PersonalTaxAppService(IRepository<PersonalTax, long> personalTaxRepository,
            IRepository<Spouse, long> spouseRepository,
            IRepository<Dependent> dependentRepository,
            IRepository<IncomeDetails, long> incomeDetailRepository,
            IRepository<Customer> customerRepository)
        {
            this.personalTaxRepository = personalTaxRepository;
            this.spouseRepository = spouseRepository;
            this.dependentRepository = dependentRepository;
            this.incomeDetailRepository = incomeDetailRepository;
            this.customerRepository = customerRepository;
        }
        public async Task SavePersonalTax(PersonalTaxDto input)
        {
            
            //getting personal tax data
            var ptaxObj = await personalTaxRepository.FirstOrDefaultAsync(x => x.FinancialYear == input.FinancialYear && x.CustomerId == input.CustomerId);
            PersonalTax personalTax = ptaxObj ?? ObjectMapper.Map<PersonalTax>(input);
            personalTax.OtherExpense = input.OtherExpense;

            //getting customer data 
            var customer = await customerRepository.FirstOrDefaultAsync(input.CustomerId);


            //getting spouse data
            var spouseObj = await spouseRepository.FirstOrDefaultAsync(input.SpouseId);
            var spouse = spouseObj ?? new Spouse();

            //getting income details
            var inputIds = input.incomeDetails.Select(x => x.Id).ToList();
            var incomeDetails = await incomeDetailRepository.GetAll()
                .Where(d => inputIds.Contains(d.Id))
                .ToListAsync();

            //getting dependents data
            var dependents = dependentRepository.GetAll().Where(x => x.CustomerId == input.CustomerId).ToList();

            //setting customer data
            customer.JobDescription = input.FilerOccupation;
            customer.DrivingLicense = input.FilersLicenseNumber;
            customer.DLIssue = input.IssueDate;
            customer.DLExpiry = input.ExpiryDate;
            customer.DLState = input.IssueState;
            customer.Code = input.ThreeDigitCode;


            //setting spouse data
            spouse.DrivingLicense = input.Spouse.DrivingLicense;
            spouse.DLIssue = input.Spouse.DLIssue;
            spouse.DLExpiry = input.Spouse.DLExpiry;
            spouse.DLState = input.Spouse.DLState;
            spouse.Code = input.Spouse.Code;

           

            //setting dependents data
            foreach(var dep in dependents)
            {
                Dependent depObj = input.Dependents.FirstOrDefault(x => x.Id == dep.Id);
                dep.Name = depObj.Name;
                dep.relation = depObj.relation;
                dep.SSN = depObj.SSN;
                dep.DateOfBirth = depObj.DateOfBirth;
            }

            //saving customer data
            await customerRepository.UpdateAsync(customer);

            //saving spouse data
            if(spouseObj != null)
            {
                await spouseRepository.UpdateAsync(spouse);
            }
            else
            {
                await spouseRepository.InsertAsync(spouse);
            }

            //saving dependents 

            List<int> dependentList = new List<int>();

            if(dependents != null && dependents.Count() > 0)
            {
                foreach(var dep in dependents)
                {
                    dependentList.Add(dep.Id);
                    await dependentRepository.UpdateAsync(dep);
                }
            }
            else
            {
                foreach (var dep in dependents)
                {
                    await dependentRepository.InsertAsync(dep);
                }
            }

            var dependentsLeft = dependents.Where(d => !dependentList.Any(x => x == d.Id)).ToList();

            foreach(var d in dependentsLeft)
            {
                await dependentRepository.InsertAsync(d);
            }


            //saving personal tax
            if (ptaxObj != null)
            {
                await personalTaxRepository.UpdateAsync(personalTax);
            }
            else
            {
                await personalTaxRepository.InsertAsync(personalTax);
            }

            //setting income details data
            List<long> incomeDetailIds = new();
            if (incomeDetails != null && incomeDetails.Count > 0)
            {
                foreach (var i in incomeDetails)
                {
                    var incomeObj = input.incomeDetails.FirstOrDefault(x => x.Id == i.Id);
                    i.IncomeDescription = incomeObj.IncomeDescription;
                    i.StateWH = incomeObj.StateWH;
                    i.FederalWH = incomeObj.FederalWH;
                    i.PersonalTaxId = personalTax.Id;
                    i.Amount = incomeObj.Amount;
                    incomeDetailIds.Add(i.Id);

                    await incomeDetailRepository.UpdateAsync(i);
                }

            }
            else
            {
                foreach (var detail in input.incomeDetails)
                {
                    detail.PersonalTaxId = personalTax.Id;
                }


                await incomeDetailRepository.InsertRangeAsync(input.incomeDetails);
            }

        }

        public async Task<PersonalTaxDto> PersonalTaxGet(PersonalTaxDto input)
        {
            var personalTax = await personalTaxRepository.FirstOrDefaultAsync(x => x.CustomerId == input.CustomerId && x.FinancialYear == input.FinancialYear );

            if(personalTax == null)
            {
                return new PersonalTaxDto();
            }

            var personaltaxDto = new PersonalTaxDto();
            personaltaxDto.CustomerId = personalTax.CustomerId;
            personaltaxDto.AccountNumber = personalTax.AccountNumber;
            personaltaxDto.FinancialYear = personalTax.FinancialYear;
            personaltaxDto.FilerOccupation = personalTax.FilerOccupation;
            personaltaxDto.BankName = personalTax.BankName;
            personaltaxDto.RoutingNumber = personalTax.RoutingNumber;
            personaltaxDto.ThreeDigitCode = personalTax.ThreeDigitCode;
            personaltaxDto.incomeDetails = await incomeDetailRepository.GetAll().Where(x => x.PersonalTaxId == personalTax.Id).ToListAsync();
            personaltaxDto.OtherExpense = personalTax.OtherExpense;
            return personaltaxDto;
        }



    }
}
